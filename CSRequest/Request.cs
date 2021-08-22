using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace CSRequest
{
    /// <summary>
    /// Main class for http requests.
    /// </summary>
    public class Request
    {
        private static readonly Lazy<HttpClient> defaultClient;
        private static readonly object defaultFactoryLock = new object();
        private static Func<string, HttpClient> defaultFactoryV2;
        private static Func<HttpClient> defaultClientFactory;
        /// <summary>
        /// Defines the default <see cref="HttpClient"/> used to execute http requests.<br/>
        /// For performance reasons, define this property only once in your app initialization.<br/>
        /// IMPORTANT: This class will NOT dispose the <see cref="HttpClient"/>. It's up to you to implement the logic to dispose it (or not).<br/>
        /// DEPRECATION NOTE: Use SetHttpClientFactory instead of this property.
        /// See: https://docs.microsoft.com/en-us/dotnet/api/system.net.http.httpclient?view=net-5.0#remarks
        /// </summary>
        [Obsolete]
        public static Func<HttpClient> DefaultClientFactory
        {
            get
            {
                return defaultClientFactory;
            }
            set
            {
                lock (defaultFactoryLock)
                {
                    defaultClientFactory = value;
                }
            }
        }

        private readonly RequestData data;
        [Obsolete]
        private Action<HttpClient> clientConfig;
        private Action<HttpResponseMessage> onSuccess;
        private Action<HttpResponseMessage> onError;
        /// <summary>
        /// The <see cref="HttpClient"/> instance used to make requests.
        /// </summary>
        public HttpClient Client { get; private set; }
        /// <summary>
        /// The request url.
        /// </summary>
        public string BaseUrl { get; }

        #region initialization

        static Request()
        {
            defaultClient = new Lazy<HttpClient>(() => new HttpClient());
        }

        /// <summary>
        /// Initialize a new instance of <see cref="Request"/>.<br/>
        /// This instance will use the client defined in the <see cref="SetHttpClientFactory(Func{string, HttpClient})"/> static function if configured. Otherwise, this class will use an internal <see cref="HttpClient"/> singleton to make requests.
        /// </summary>
        /// <param name="baseUrl">The base url of the request.</param>
        /// <remarks></remarks>
        public Request(string baseUrl)
        {
            BaseUrl = baseUrl;
            data = new RequestData(baseUrl);
            Client = defaultClientFactory?.Invoke() ?? defaultFactoryV2?.Invoke(baseUrl) ?? defaultClient.Value;
        }

        /// <summary>
        /// Initialize a new instance of <see cref="Request"/> that will use the provided <see cref="HttpClient"/> to make requests.
        /// </summary>
        /// <param name="httpClient">The client that will be used to make requests.</param>
        /// <param name="baseUrl">The base url of the request.</param>
        public Request(HttpClient httpClient, string baseUrl) : this(baseUrl: baseUrl)
        {
            Client = httpClient;
        }

        /// <summary>
        /// Initialize a new instance of <see cref="Request"/>.<br/>
        /// This instance will use the client defined in the <see cref="SetHttpClientFactory(Func{string, HttpClient})"/> static function if configured. Otherwise, this class will use an internal <see cref="HttpClient"/> singleton to make requests.
        /// </summary>
        public Request() : this(baseUrl: null)
        {
        }

        /// <summary>
        /// Sets the function that will be called to create a <see cref="HttpClient"/> internally. <br/>
        /// For performance reasons, define this property only once in your app initialization.<br/>
        /// IMPORTANT: This class will NOT dispose the <see cref="HttpClient"/>. It's up to you to implement the logic to dispose it (or not).<br/>
        /// See: https://docs.microsoft.com/en-us/dotnet/api/system.net.http.httpclient?view=net-5.0#remarks
        /// </summary>
        /// <param name="factoryFunction">
        /// The function that creates the <see cref="HttpClient"/> to execute requests.<br/>
        /// The base url defined in the constructor will be the parameter of this function. If no base url is provided, the parameter will be null.
        /// </param>
        public static void SetHttpClientFactory(Func<string, HttpClient> factoryFunction)
        {
            lock(defaultFactoryLock)
            {
                defaultFactoryV2 = factoryFunction;
            }
        }

        #endregion

        #region fluent interface

        /// <summary>
        /// Overrides the <see cref="DefaultClientFactory"/> function.
        /// </summary>
        /// <param name="clientInjector">A function to inject a <see cref="Client"/>/>.></param>
        /// <returns>Fluent.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        [Obsolete("Use the constructor overload to inject the HttpClient")]
        public Request InjectClient(Func<HttpClient> clientInjector)
        {
            if (clientInjector == null) throw new ArgumentNullException(nameof(clientInjector));
            this.Client = clientInjector.Invoke();
            return this;
        }

        /// <summary>
        /// Configure the injected <see cref="Client"/> used to make requests./>
        /// </summary>
        /// <param name="clientConfig">The configuration function.</param>
        /// <returns>Fluent.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        [Obsolete("This method will be removed in future releases because changes in HttpClient is not thread safe. See: https://docs.microsoft.com/en-us/dotnet/api/system.net.http.httpclient?view=net-5.0#remarks")]
        public Request ConfigureClient(Action<HttpClient> clientConfig)
        {
            this.clientConfig = clientConfig ?? throw new ArgumentNullException(nameof(clientConfig));
            return this;
        }

        /// <summary>
        /// Adds segments to the url. ["a", "b", "c"] will be evaluated as to a/b/c.
        /// </summary>
        /// <param name="segments">The segments to add to the request url.</param>
        /// <returns>Fluent</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public Request WithSegments(params string[] segments)
        {
            if (segments == null) throw new ArgumentNullException(nameof(segments));
            data.SegmentList.AddRange(segments);
            return this;
        }

        /// <summary>
        /// Add headers to the request. new { foo = "bar" } will be evaluated to foo: bar.<br/>
        /// Underscores (_) are translated to dashes (-), like: new { Content_type = "plain/text" } will be evaluated to Content-type: plain/text.
        /// </summary>
        /// <param name="header">An object that representes the headers.</param>
        /// <returns>Fluent.</returns>
        public Request WithHeader(object header)
        {
            data.Header = header;
            return this;
        }

        /// <summary>
        /// Adds a bearer authorization header token.
        /// </summary>
        /// <param name="token">The bearer token. This method doesn't encode the token.</param>
        /// <returns>Fluent.</returns>
        public Request AddOABearerToken(string token)
        {
            data.BearerToken = token;
            return this;
        }

        /// <summary>
        /// Adds a stream to be uploaded. Works like a html's file input element. The request content-type is set to multipart/form-data.
        /// </summary>
        /// <param name="stream">The data stream to be uploaded.</param>
        /// <param name="fileName">A file name attached to the stream. If none is provided, a random name will be created.</param>
        /// <returns>Fluent.</returns>
        public Request AddFormFile(Stream stream, string fileName = null)
        {
            var name = fileName ?? Path.GetRandomFileName();
            data.FormFiles.Add(name, stream);
            return this;
        }

        /// <summary>
        /// Adds multiple streams to be uploaded. Works like a html's file input element. The request content-type is set to multipart/form-data.
        /// </summary>
        /// <param name="streams">The data streams to be uploaded. Random file names will be attached to each stream.</param>
        /// <returns>Fluent.</returns>
        public Request AddFormFile(IEnumerable<Stream> streams)
        {
            foreach (var stream in streams)
            {
                AddFormFile(stream);
            }
            return this;
        }

        /// <summary>
        /// Appends a query string to the url. new { foo="bar", bar="foo"} will be evaluated to ?foo=bar&amp;bar=foo.
        /// </summary>
        /// <param name="query">The object to be converted into a query string.</param>
        /// <returns>Fluent.</returns>
        public Request WithQuery(object query)
        {
            data.Query = query;
            return this;
        }

        /// <summary>
        /// Adds form data. The request content-type is set to multipart/form-data.
        /// </summary>
        /// <param name="formData">The object to be converted into form data.</param>
        /// <returns>Fluent.</returns>
        public Request WithFormData(object formData)
        {
            data.FormData = formData;
            return this;
        }

        /// <summary>
        /// Set the request cookies.
        /// </summary>
        /// <param name="cookies">An object that representes the key/values of the cookies.</param>
        /// <returns>Fluent.</returns>
        public Request WithCookies(object cookies)
        {
            data.CookieObj = cookies;
            return this;
        }

        /// <summary>
        /// Set a callback function in case you have a successful status code response.
        /// </summary>
        /// <param name="successCallback">A callback to be executed.</param>
        /// <returns>Fluent.</returns>
        public Request OnSuccess(Action<HttpResponseMessage> successCallback)
        {
            onSuccess = successCallback;
            return this;
        }

        /// <summary>
        /// Set a callback function in case you have a failure status code response.
        /// </summary>
        /// <param name="errorCallback">A callback to be executed.</param>
        /// <returns>Fluent.</returns>
        public Request OnError(Action<HttpResponseMessage> errorCallback)
        {
            onError = errorCallback;
            return this;
        }

        /// <summary>
        /// Adds a json object in the request body. Sets the content-type to application/json.
        /// </summary>
        /// <param name="body">The object to be converted into a json object.</param>
        /// <remarks>Overrides <see cref="WithFormData"/> and <see cref="AddFormFile(Stream, string)"/> functions.</remarks>
        /// <returns>fluent.</returns>
        public Request WithJsonBody(object body)
        {
            data.JsonBody = body;
            return this;
        }

        #endregion

        #region request execution

        /// <summary>
        /// Performs a get request.
        /// </summary>
        /// <returns>A <see cref="HttpResponseMessage"/>.</returns>
        public Task<HttpResponseMessage> GetAsync() => RequestAsync(HttpMethod.Get);

        /// <summary>
        /// Performs a post request.
        /// </summary>
        /// <returns>A <see cref="HttpResponseMessage"/>.</returns>
        public Task<HttpResponseMessage> PostAsync() => RequestAsync(HttpMethod.Post);

        /// <summary>
        /// Performs a put request.
        /// </summary>
        /// <returns>A <see cref="HttpResponseMessage"/>.</returns>
        public Task<HttpResponseMessage> PutAsync() => RequestAsync(HttpMethod.Put);

        /// <summary>
        /// Performs a patch request.
        /// </summary>
        /// <returns>A <see cref="HttpResponseMessage"/>.</returns>
        public Task<HttpResponseMessage> PatchAsync() => RequestAsync(HttpMethod.Patch);

        /// <summary>
        /// Performs a delete request.
        /// </summary>
        /// <returns>A <see cref="HttpResponseMessage"/>.</returns>
        public Task<HttpResponseMessage> DeleteAsync() => RequestAsync(HttpMethod.Delete);

        /// <summary>
        /// Performs a syncronous get request.
        /// </summary>
        /// <returns>A <see cref="HttpResponseMessage"/>.</returns>
        public HttpResponseMessage Get() => GetAsync().Result;

        /// <summary>
        /// Performs a syncronous post request.
        /// </summary>
        /// <returns>A <see cref="HttpResponseMessage"/>.</returns>
        public HttpResponseMessage Post() => PostAsync().Result;

        /// <summary>
        /// Performs a syncronous put request.
        /// </summary>
        /// <returns>A <see cref="HttpResponseMessage"/>.</returns>
        public HttpResponseMessage Put() => PutAsync().Result;

        /// <summary>
        /// Performs a syncronous patch request.
        /// </summary>
        /// <returns>A <see cref="HttpResponseMessage"/>.</returns>
        public HttpResponseMessage Patch() => PatchAsync().Result;

        /// <summary>
        /// Performs a syncronous delete request.
        /// </summary>
        /// <returns>A <see cref="HttpResponseMessage"/>.</returns>
        public HttpResponseMessage Delete() => DeleteAsync().Result;

        private async Task<HttpResponseMessage> RequestAsync(HttpMethod method)
        {
            if (Client == null)
                throw new Exception($"You must define a {nameof(HttpClient)} in the constructor or statically in {nameof(SetHttpClientFactory)} method.");

            clientConfig?.Invoke(Client);

            using HttpRequestMessage request = data.BuildRequest(method);
            try
            {
                var response = await Client.SendAsync(request).ConfigureAwait(false);
                if (response.IsSuccessStatusCode) onSuccess?.Invoke(response);
                else onError?.Invoke(response);
                return response;
            }
            catch (Exception)
            {
                throw;
            }
            
        }

        #endregion

    }

}
