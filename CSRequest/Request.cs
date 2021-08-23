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

}
