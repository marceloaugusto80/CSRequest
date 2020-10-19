using System;
using System.Net.Http;
using Xunit;

namespace CSRequest.Test.Helpers
{

    public class ServerFixture : IDisposable
    {
        private readonly HttpClient client;

        public ServerFixture()
        {
            client = new HttpClient()
            {
                BaseAddress = new Uri("http://postman-echo.com/")
            };
            //client.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:81.0) Gecko/20100101 Firefox/81.0");
            //client.DefaultRequestHeaders.Add("accept-encoding", "gzip, deflate");
            //client.DefaultRequestHeaders.Add("connection", "keep-alive");

            Request.DefaultClientFactory = () => client;

        }

        public void Dispose()
        {
            client.Dispose();
        }
    }

    [CollectionDefinition(nameof(ServerFixtureCollection))]
    public class ServerFixtureCollection : ICollectionFixture<ServerFixture>
    {
    }
}
