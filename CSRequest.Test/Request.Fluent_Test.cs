using CSRequest.Test.Helpers;
using FluentAssertions;
using System;
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
        public async Task WithBasicAuthentication_test()
        {
            var echo = await new Request(echoUrl)
                .WithSegments("get")
                .WithBasicAuthentication("myuser", "mypass")
                .Get()
                .ReadJsonAsync<EchoResponse>();

            echo.Headers.Should().Contain("authorization", "Basic bXl1c2VyOm15cGFzcw==");
        }

        [Fact]
        public async Task WithBearerTokenAuthentication_test()
        {
            var echo = await new Request(echoUrl)
                .WithSegments("get")
                .WithBearerTokenAuthentication("my-token")
                .Get()
                .ReadJsonAsync<EchoResponse>();

            echo.Headers.Should().Contain("authorization", "Bearer my-token");
        }

        [Fact]
        public void WithCookies_test()
        {
            var echo = new Request(echoUrl)
               .WithSegments("cookies")
               .WithCookies(new { k1 = "v1", k2 = "v2" })
               .Get()
               .ReadJson<EchoResponse>();

            echo.Cookies.Should().Contain("k1", "v1").And.Contain("k2", "v2");
        }

        [Fact]
        public void WithFormData_test()
        {
            var echo = new Request(echoUrl)
                .WithSegments("post")
                .WithFormData(new { k1 = "v1", k2 = "v2" })
                .Post()
                .ReadJson<EchoResponse>();

            echo.Form.Should().Contain("k1", "v1").And.Contain("k2", "v2");
        }

        [Fact]
        public async Task AddFormFile_test()
        {
            var echo = await new Request(echoUrl)
               .WithSegments("post")
               .AddFormFile(Generators.GenerateStream(), "file1", "file1.txt")
               .Post()
               .ReadJsonAsync<EchoResponse>();

            echo.Files.Should().ContainKey("file1.txt");
        }

        [Fact]
        public void AddFormFile_multiple_files_test()
        {
            var echo = new Request(echoUrl)
               .WithSegments("post")
               .AddFormFile(Generators.GenerateStream(3))
               .Post()
               .ReadJson<EchoResponse>();

            echo.Files.Should().HaveCount(3);
        }

        [Fact]
        public void WithHeader_test()
        {
            var echo = new Request(echoUrl)
                .WithSegments("get")
                .WithHeader(new { k1 = "v1", k2 = "v2" })
                .Get()
                .ReadJson<EchoResponse>();

            echo.Headers.Should().Contain("k1", "v1").And.Contain("k2", "v2");
        }

        [Fact]
        public void WithJsonBody_test()
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

        [Fact]
        public void WithQuery()
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

    }
}
