using System;
using System.Net.Http;

namespace CSRequest
{
    /// <summary>
    /// Global request configurations for <see cref="Request"/> class.
    /// </summary>
    public static class RequestConfiguration
    {
        private static readonly Lazy<HttpClient> defaultClient;
        private static readonly object defaultFactoryLock = new object();
        private static Func<string, HttpClient> clientFactory;

        static RequestConfiguration()
        {
            defaultClient = new Lazy<HttpClient>(() => new HttpClient());
        }

        internal static HttpClient GetClient(string baseUrl)
        {
            return clientFactory?.Invoke(baseUrl) ?? defaultClient.Value;
        }

        /// <summary>
        /// Sets the function that will be called to create a <see cref="HttpClient"/> internally. <br/>
        /// Although it's thread safe, for performance reasons, define this property only once in your app initialization.<br/>
        /// IMPORTANT: This class will NOT dispose the <see cref="HttpClient"/>. It's up to you to implement the logic to dispose it (or not).<br/>
        /// See: https://docs.microsoft.com/en-us/dotnet/api/system.net.http.httpclient?view=net-5.0#remarks
        /// </summary>
        /// <param name="factoryFunction">
        /// The function that creates the <see cref="HttpClient"/> to execute requests.<br/>
        /// The base url defined in the constructor will be the parameter of this function. If no base url is provided, the parameter will be null.
        /// </param>
        public static void SetHttpClientFactory(Func<string, HttpClient> factoryFunction)
        {
            lock (defaultFactoryLock)
            {
                clientFactory = factoryFunction;
            }
        }

    }
}
