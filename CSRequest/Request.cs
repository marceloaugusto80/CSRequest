﻿using CSRequest.Internal;
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
    public partial class Request
    {
        private Action<HttpResponseMessage> onSuccess;
        private Action<HttpResponseMessage> onError;
        private readonly string baseUrl;
        private readonly List<IRequestTransform> transforms;
        private readonly HttpClient client;

        /// <summary>
        /// Initialize a new instance of <see cref="Request"/> class.<br/>
        /// This instance will use an internal reusable HttpClient singleton or the one defined in the <see cref="RequestConfiguration.SetHttpClientFactory(Func{string, HttpClient})"/> static function.
        /// </summary>
        /// <param name="baseUrl">The base url of the request.</param>
        /// <remarks></remarks>
        public Request(string baseUrl) : this(baseUrl, RequestConfiguration.GetClient(baseUrl))
        {
        }

        /// <summary>
        /// Initialize a new instance of <see cref="Request"/> class that will use the provided <see cref="HttpClient"/> to make requests.
        /// </summary>
        /// <param name="baseUrl">The base url of the request.</param>
        /// <param name="httpClient">The client that will be used to make requests.</param>
        public Request(string baseUrl, HttpClient httpClient)
        {
            transforms = new List<IRequestTransform>();
            this.baseUrl = baseUrl;
            client = httpClient;
        }

        private async Task<HttpResponseMessage> RequestAsync(HttpMethod method)
        {
            if (client == null)
            {
                throw new Exception(
                    $"You must define a {nameof(HttpClient)} in the constructor or statically in {nameof(RequestConfiguration.SetHttpClientFactory)} method.");
            }

            try
            {

                using HttpRequestMessage request = new HttpRequestMessage(method, baseUrl);
                foreach (var transform in transforms)
                {
                    transform.Transform(request);
                }

                var response = await client.SendAsync(request).ConfigureAwait(false);
                
                if (response.IsSuccessStatusCode) onSuccess?.Invoke(response);
                else onError?.Invoke(response);
                
                return response;
            }
            catch (Exception)
            {
                throw;
            }
            
        }

    }

    public partial class Request
    {

        /// <summary>
        /// Adds segments to the url. ["a", "b", "c"] will be evaluated as to a/b/c.
        /// </summary>
        /// <param name="segments">The segments to add to the request url.</param>
        /// <returns>Fluent</returns>
        /// <exception cref="ArgumentNullException"/>
        public Request WithSegments(params string[] segments)
        {
            if (segments == null) throw new ArgumentNullException(nameof(segments));
            transforms.Add(new UrlSegmentsRequestTransform(segments));
            return this;
        }

        /// <summary>
        /// Add headers to the request. new { foo = "bar" } will be evaluated to foo: bar.<br/>
        /// Underscores (_) are translated to dashes (-), like: new { Content_type = "plain/text" } will be evaluated to Content-type: plain/text.
        /// </summary>
        /// <param name="header">An object that representes the headers.</param>
        /// <returns>Fluent.</returns>
        /// <exception cref="ArgumentNullException"/>
        public Request WithHeader(object header)
        {
            if (header == null) throw new ArgumentNullException(nameof(header));
            transforms.Add(new HeaderRequestTransform(header));
            return this;
        }

        /// <summary>
        /// Adds a bearer authorization header token.
        /// </summary>
        /// <param name="token">The bearer token. This method doesn't encode the token.</param>
        /// <returns>Fluent.</returns>
        /// <exception cref="ArgumentNullException"/>
        public Request AddOABearerToken(string token)
        {
            if (token == null) throw new ArgumentNullException(nameof(token));
            transforms.Add(new BearerTokenRequestTransform(token));
            return this;
        }

        /// <summary>
        /// Adds a stream to be uploaded. Works like a html's file input element. The request content-type is set to multipart/form-data.
        /// </summary>
        /// <param name="stream">The data stream to be uploaded.</param>
        /// <param name="fieldName">The name of the form data field related to the file. If none is provided, a random field name will be created.</param>
        /// <param name="fileName">A file name attached to the stream. If none is provided, a random name will be created.</param>
        /// <returns>Fluent.</returns>
        /// <exception cref="ArgumentNullException"/>
        public Request AddFormFile(Stream stream, string fieldName = null, string fileName = null)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            var name = fileName ?? Path.GetRandomFileName();
            var field = fieldName ?? name;
            transforms.Add(new FormFileRequestTransform(stream, field, name));
            return this;
        }

        /// <summary>
        /// Adds multiple streams to be uploaded. Works like a html's file input element. The request content-type is set to multipart/form-data.<br/>
        /// File names and field names associated with the files will be random.
        /// </summary>
        /// <param name="streams">The data streams to be uploaded. Random file names will be attached to each stream.</param>
        /// <returns>Fluent.</returns>
        /// <exception cref="ArgumentNullException"/>
        public Request AddFormFile(IEnumerable<Stream> streams)
        {
            if (streams == null) throw new ArgumentNullException(nameof(streams));
            int counter = 0;
            foreach (var stream in streams)
            {
                AddFormFile(stream, $"file{counter++}");
            }
            return this;
        }

        /// <summary>
        /// Appends a query string to the url. new { foo="bar", bar="foo"} will be evaluated to ?foo=bar&amp;bar=foo.
        /// </summary>
        /// <param name="query">The object to be converted into a query string.</param>
        /// <returns>Fluent.</returns>
        /// <exception cref="ArgumentNullException"/>
        public Request WithQuery(object query)
        {
            if (query == null) throw new ArgumentNullException(nameof(query));
            transforms.Add(new UrlQueryRequestTransform(query));
            return this;
        }

        /// <summary>
        /// Adds form data. The request content-type is set to multipart/form-data.
        /// </summary>
        /// <param name="formData">The object to be converted into form data.</param>
        /// <returns>Fluent.</returns>
        /// <exception cref="ArgumentNullException"/>
        public Request WithFormData(object formData)
        {
            if (formData == null) throw new ArgumentNullException(nameof(formData));
            transforms.Add(new FormDataRequestTransform(formData));
            return this;
        }

        /// <summary>
        /// Set the request cookies.
        /// </summary>
        /// <param name="cookies">An object that representes the key/values of the cookies.</param>
        /// <returns>Fluent.</returns>
        /// <exception cref="ArgumentNullException"/>
        public Request WithCookies(object cookies)
        {
            if (cookies == null) throw new ArgumentNullException(nameof(cookies));
            transforms.Add(new CoockieRequestTransform(cookies));
            return this;
        }

        /// <summary>
        /// Adds a json object in the request body. Sets the content-type to application/json.
        /// </summary>
        /// <param name="body">The object to be converted into a json object.</param>
        /// <remarks>Overrides <see cref="WithFormData"/> and <see cref="AddFormFile(Stream, string, string)"/> functions.</remarks>
        /// <returns>fluent.</returns>
        /// <exception cref="ArgumentNullException"/>
        public Request WithJsonBody(object body)
        {
            if (body == null) throw new ArgumentNullException(nameof(body));
            transforms.Add(new JsonContentRequestTransform(body));
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

    }

    public partial class Request
    {

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
    }
}
