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

        private Func<HttpClient> clientInjector;
        private Action<HttpClient> clientConfig;
        private Action<HttpResponseMessage> onSuccess;
        private Action<HttpResponseMessage> onError;
        
        public RequestData Data { get; }

        public Request()
        {
            Data = new RequestData();
        }

        #region fluent interface

        /// <summary>
        /// Overrides the <see cref="DefaultClientFactory"/> function.
        /// </summary>
        /// <param name="clientInjector">The factory funcion.</param>
        /// <returns>This <see cref="Request"/> instance.</returns>
        public Request InjectClient(Func<HttpClient> clientInjector)
        {
            this.clientInjector = clientInjector ?? throw new ArgumentNullException(nameof(clientInjector));
            return this;
        }

        public Request ConfigureClient(Action<HttpClient> clientConfig)
        {
            this.clientConfig = clientConfig;
            return this;
        }

        public Request WithSegments(params string[] segments)
        {
            if (segments == null) throw new ArgumentNullException(nameof(segments));
            Data.SegmentList.AddRange(segments);
            return this;
        }

        public Request WithHeader(object header)
        {
            Data.Header = header;
            return this;
        }

        public Request AddOABearerToken(string token)
        {
            Data.BearerToken = token;
            return this;
        }

        public Request AddFormFile(Stream stream, string fileName = null)
        {
            var name = fileName ?? Path.GetRandomFileName();
            Data.FormFiles.Add(name, stream);
            return this;
        }

        public Request AddFormFile(IEnumerable<Stream> streams)
        {
            foreach (var stream in streams)
            {
                AddFormFile(stream);
            }
            return this;
        }

        public Request WithQuery(object query)
        {
            Data.Query = query;
            return this;
        }

        public Request WithFormData(object formData)
        {
            Data.FormData = formData;
            return this;
        }

        public Request WithCookies(object cookies)
        {
            Data.CookieObj = cookies;
            return this;
        }

        public Request OnSuccess(Action<HttpResponseMessage> successCallback)
        {
            onSuccess = successCallback;
            return this;
        }

        public Request OnError(Action<HttpResponseMessage> errorCallback)
        {
            onError = errorCallback;
            return this;
        }

        public Request WithJsonBody(object body)
        {
            Data.JsonBody = body;
            return this;
        }

        #endregion

        #region request execution

        public Task<HttpResponseMessage> GetAsync() => RequestAsync(HttpMethod.Get);

        public Task<HttpResponseMessage> PostAsync() => RequestAsync(HttpMethod.Post);

        public Task<HttpResponseMessage> PutAsync() => RequestAsync(HttpMethod.Put);
        
        public Task<HttpResponseMessage> PatchAsync() => RequestAsync(HttpMethod.Patch);
        
        public Task<HttpResponseMessage> DeleteAsync() => RequestAsync(HttpMethod.Delete);

        public HttpResponseMessage Get() => GetAsync().Result;

        public HttpResponseMessage Post() => PostAsync().Result;
        
        public HttpResponseMessage Put() => PutAsync().Result;
        
        public HttpResponseMessage Patch() => PatchAsync().Result;
        
        public HttpResponseMessage Delete() => DeleteAsync().Result;

        private async Task<HttpResponseMessage> RequestAsync(HttpMethod method)
        {
            var clientFactory = clientInjector ?? defaultClientFactory;

            if (clientFactory == null)
                throw new Exception($"You must define a HttpClient in {nameof(InjectClient)} method or {nameof(DefaultClientFactory)} property.");

            HttpClient client = clientFactory.Invoke();
            if (client == null) throw new Exception("Client resolved to null. Check how you're injecting the HttpClient instance.");

            clientConfig?.Invoke(client);

            using HttpRequestMessage request = Data.BuildRequest(method);

            var response = await client.SendAsync(request).ConfigureAwait(false);
            
            if (response.IsSuccessStatusCode) onSuccess?.Invoke(response);
            else onError?.Invoke(response);

            return response;
        }

        #endregion

    }

}
