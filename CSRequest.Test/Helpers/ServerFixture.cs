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
