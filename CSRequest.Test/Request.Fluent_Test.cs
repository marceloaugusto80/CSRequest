using CSRequest.Test.Helpers;
using FluentAssertions;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace CSRequest
{
    public class Request_Fluent_Test
    {
        private readonly string echoUrl;

        public Request_Fluent_Test()
        {
            echoUrl = @"http://postman-echo.com";
        }

        [Fact]
        public void Request_add_headers()
        {
            var echo = new Request(echoUrl)
                .WithSegments("get")
                .WithHeader(new { k1 = "v1", k2 = "v2" })
                .Get()
                .ReadJson<EchoResponse>();

            echo.Headers.Should().Contain("k1", "v1").And.Contain("k2", "v2");
        }

        [Fact]
        public void Request_add_query_string()
        {
            var echo = new Request(echoUrl)
                .WithSegments("get")
                .WithQuery(new { k1 = "v1", k2 = "v2" })
                .Get()
                .ReadJson<EchoResponse>();

            echo.Args.Should().Contain("k1", "v1").And.Contain("k2", "v2");
        }

        [Fact]
        public async Task Request_handle_success()
        {
            var response = await new Request(echoUrl)
                .WithSegments("status", "200")
                .OnSuccess(resp => resp.Should().NotBeNull())
                .GetAsync();

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Request_handle_errors()
        {
            var response = await new Request(echoUrl)
                .WithSegments("status", "400")
                .OnError(resp => resp.Should().NotBeNull())
                .GetAsync();

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public void Request_send_form_data()
        {
            var echo = new Request(echoUrl)
                .WithSegments("post")
                .WithFormData(new { k1 = "v1", k2 = "v2" })
                .Post()
                .ReadJson<EchoResponse>();

            echo.Form.Should().Contain("k1", "v1").And.Contain("k2", "v2");
        }

        [Fact]
        public async Task Request_send_form_file()
        {
            var echo = await new Request(echoUrl)
               .WithSegments("post")
               .AddFormFile(Generators.GenerateStream(), "file1", "file1.txt")
               .Post()
               .ReadJsonAsync<EchoResponse>();

            echo.Files.Should().ContainKey("file1.txt");
        }

        [Fact]
        public void Request_send_multiple_form_files()
        {
            var echo = new Request(echoUrl)
               .WithSegments("post")
               .AddFormFile(Generators.GenerateStream(3))
               .Post()
               .ReadJson<EchoResponse>();

            echo.Files.Should().HaveCount(3);
        }

        [Fact]
        public void Request_send_cookies()
        {
            var echo = new Request(echoUrl)
               .WithSegments("cookies")
               .WithCookies(new { k1 = "v1", k2 = "v2" })
               .Get()
               .ReadJson<EchoResponse>();

            echo.Cookies.Should().Contain("k1", "v1").And.Contain("k2", "v2");
        }

        [Fact]
        public void Request_send_json_body()
        {
            var expected = new { someString = "foo", someNumber = 42 };

            var actual = new Request(echoUrl)
               .WithSegments("post")
               .WithJsonBody(expected)
               .Post()
               .ReadJson<EchoResponse>()
               .Json;

            actual.Should().Contain("someString", "foo").And.Contain("someNumber", 42);
        }
    }
}
