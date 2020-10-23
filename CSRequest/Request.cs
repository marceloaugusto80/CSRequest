using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace CSRequest
{
    public class Request
    {
        private static readonly object defaultFactoryLock = new object();
        private static Func<HttpClient> defaultClientFactory;
        /// <summary>
        /// Defines the default <see cref="HttpClient"/> used to execute http requests./>
        /// </summary>
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
        private Func<HttpClient> clientInjector;
        private Action<HttpClient> clientConfig;
        private Action<HttpResponseMessage> onSuccess;
        private Action<HttpResponseMessage> onError;

        public Request()
        {
            data = new RequestData();
        }

        #region fluent interface

        /// <summary>
        /// Overrides the <see cref="DefaultClientFactory"/> function.
        /// </summary>
        /// <param name="clientInjector">A function to inject a <see cref="HttpClient/>"/>.></param>
        /// <returns>Fluent.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public Request InjectClient(Func<HttpClient> clientInjector)
        {
            this.clientInjector = clientInjector ?? throw new ArgumentNullException(nameof(clientInjector));
            return this;
        }

        /// <summary>
        /// Configure the injected <see cref="HttpClient"/> used to make request./>
        /// </summary>
        /// <param name="clientConfig">The configuration function.</param>
        /// <returns>Fluent.</returns>
        /// <exception cref="ArgumentNullException"></exception>
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
        /// Add headers to the request. new { foo = "bar" } will be evaluated to foo=bar.
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
        /// Appends a query string to the url. new { foo="bar", bar="foo"} will be evaluated to ?foo=bar&bar=foo.
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
        /// <remarks>Overrides <see cref="WithFormData"/> and <see cref="AddFormFile"/> functions.</remarks>
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
            var clientFactory = clientInjector ?? defaultClientFactory;

            if (clientFactory == null)
                throw new Exception($"You must define a HttpClient in {nameof(InjectClient)} method or {nameof(DefaultClientFactory)} property.");

            HttpClient client = clientFactory.Invoke();
            if (client == null) throw new Exception("Client resolved to null. Check how you're injecting the HttpClient instance.");

            clientConfig?.Invoke(client);

            using HttpRequestMessage request = data.BuildRequest(method);

            var response = await client.SendAsync(request).ConfigureAwait(false);
            
            if (response.IsSuccessStatusCode) onSuccess?.Invoke(response);
            else onError?.Invoke(response);

            return response;
        }

        #endregion

    }

}
