using CSRequest.Internal;
using System;
using System.Collections.Generic;
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
        internal List<IRequestTransform> Transforms { get; }
        private readonly HttpClient client;

        /// <summary>
        /// Initialize a new instance of <see cref="Request"/> class that will use the provided <see cref="HttpClient"/> to make requests.
        /// </summary>
        /// <param name="baseUrl">The base url of the request.</param>
        /// <param name="httpClient">The client that will be used to make requests.</param>
        public Request(string baseUrl, HttpClient httpClient)
        {
            Transforms = new List<IRequestTransform>();
            this.baseUrl = baseUrl;
            client = httpClient;
        }

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
        /// Initialize a new instance of <see cref="Request"/> class.<br/>
        /// Make sure <see cref="HttpClient.BaseAddress"/> is defined in the client injected via <see cref="RequestConfiguration.SetHttpClientFactory(Func{string, HttpClient})"/>
        /// </summary>
        public Request() : this(string.Empty)
        { }

        /// <summary>
        /// Initialize a new instance of <see cref="Request"/> class.<br/>
        /// Make sure <see cref="HttpClient.BaseAddress"/> is defined in the provided client.
        /// </summary>
        /// <param name="client">The client used to make requests.</param>
        public Request(HttpClient client) : this(string.Empty, client)
        { }

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
        /// Set a callback function in case you have a failure status code response.<br/>
        /// IMPORTANT: This callback will not be called if the request execution throws an exception. Use use try/catch to deal with any eventual exception.
        /// </summary>
        /// <param name="errorCallback">A callback to be executed.</param>
        /// <returns>Fluent.</returns>
        public Request OnError(Action<HttpResponseMessage> errorCallback)
        {
            onError = errorCallback;
            return this;
        }

        private async Task<HttpResponseMessage> RequestAsync(HttpMethod method)
        {
            if (client == null)
            {
                throw new Exception(
                    $"You must define a {nameof(HttpClient)} in the constructor or statically in {nameof(RequestConfiguration.SetHttpClientFactory)} method.");
            }

            var resolvedUri = string.IsNullOrEmpty(baseUrl) ? 
                client.BaseAddress : 
                new Uri(baseUrl, UriKind.Absolute);

            if(resolvedUri == null)
            {
                throw new Exception($"Could not resolve request url. Set the ulr in the constructor or the in the injected HttpClient BaseUri property.");
            }

            try
            {
                using HttpRequestMessage request = new HttpRequestMessage()
                {
                    RequestUri = resolvedUri,
                    Method = method
                };
                foreach (var transform in Transforms)
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

}
