using CSRequest.Test.Helpers;
using System.Net.Http;

namespace CSRequest
{
    public class Request_Test
    {
        [Fact]
        public async Task Initialization_inject_client()
        {
            const string url = @"http://postman-echo.com";
            using var client = new HttpClient()
            {
                BaseAddress = new Uri(url, UriKind.Absolute)
            };

            using var response = await new Request(client).WithSegments("get").GetAsync();
            var echo = await response.ReadJsonAsync<EchoResponse>();

            echo.Url.Should().Be(url + "/get");
        }

        [Fact]
        public async Task Initialization_inject_url_and_client()
        {
            const string url = @"http://postman-echo.com";
            using var client = new HttpClient();

            using var response = await new Request(url, client).WithSegments("get").GetAsync();
            var echo = await response.ReadJsonAsync<EchoResponse>();

            echo.Url.Should().Be(url + "/get");
        }

        [Fact]
        public async Task Initialization_inject_url()
        {
            const string url = @"http://postman-echo.com";

            using var response = await new Request(url).WithSegments("get").GetAsync();
            var echo = await response.ReadJsonAsync<EchoResponse>();

            echo.Url.Should().Be(url + "/get");
        }

        [Fact]
        public async Task Initialization_inject_nothing()
        {
            Func<Task> action = () => new Request().WithSegments("get").GetAsync();

            await action.Should().ThrowAsync<Exception>().WithMessage("Could not resolve request url*");
        }

        [Fact]
        public async Task Initialization_null_client()
        {
            HttpClient client = null;
            Func<Task> action = () => new Request(client).WithSegments("get").GetAsync();

            await action.Should().ThrowAsync<Exception>().WithMessage("You must define a*");
        }
    }
}
