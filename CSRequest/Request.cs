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

        private readonly RequestData data;
        private Func<HttpClient> clientInjector;
        private Action<HttpClient> clientConfig;
        private Action<HttpResponseMessage> onSuccess;
        private Action<HttpResponseMessage> onError;
        private Action<Dictionary<string, string>> onGetCookies;

        public Request()
        {
            data = new RequestData();
        }

        public Request InjectClient(Func<HttpClient> clientResolver)
        {
            this.clientInjector = clientResolver;
            return this;
        }

        public Request ConfigureClient(Action<HttpClient> clientConfig)
        {
            this.clientConfig = clientConfig;
            return this;
        }

        public Request WithSegments(params string[] segments)
        {
            data.SegmentList.AddRange(segments);
            return this;
        }

        public Request WithHeader(object header)
        {
            data.Header = header;
            return this;
        }

        public Request AddOABearerToken(string token)
        {
            data.BearerToken = token;
            return this;
        }

        public Request AddFormFile(Stream stream, string fileName = null)
        {
            var name = fileName ?? Path.GetRandomFileName();
            data.FormFiles.Add(name, stream);
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
            data.Query = query;
            return this;
        }

        public Request WithFormData(object formData)
        {
            data.FormData = formData;
            return this;
        }

        public Request WithCookies(object cookies)
        {
            data.CookieObj = cookies;
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

        public Request OnGetCookies(Action<Dictionary<string, string>> onGetCookiesCallback)
        {
            onGetCookies = onGetCookiesCallback;
            return this;
        }

        private async Task<HttpResponseMessage> RequestAsync(HttpMethod method)
        {
            HttpClient client = clientInjector == null ? defaultClientFactory.Invoke() : clientInjector.Invoke();
            if (client == null) throw new Exception("Client cannot be null.");

            clientConfig?.Invoke(client);

            using HttpRequestMessage request = data.BuildRequest(method);

            try
            {
                var response = await client.SendAsync(request).ConfigureAwait(false);

                if (response.IsSuccessStatusCode) onSuccess?.Invoke(response);
                else onError?.Invoke(response);

                if (onGetCookies != null && response.Headers.Contains("Set-Cookie"))
                {
                    var cookies = response.Headers
                        .Where(h => h.Key == "Set-Cookie")
                        .ToDictionary(kv => kv.Key, kv => string.Join(',', kv.Value));
                    if (cookies.Count > 0) onGetCookies(cookies);
                }

                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Task<HttpResponseMessage> GetAsync() => RequestAsync(HttpMethod.Get);

        public Task<HttpResponseMessage> PostAsync() => RequestAsync(HttpMethod.Post);

        public HttpResponseMessage Get() => GetAsync().Result;

        public HttpResponseMessage Post() => PostAsync().Result;


    }

}
