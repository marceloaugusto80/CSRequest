using CSRequest.Test.Helpers;
using FluentAssertions;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace CSRequest.Test
{
    public class Request_Test
    {
        private readonly string echoUrl;

        public Request_Test()
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
        public void Request_send_form_file()
        {
            var echo = new Request(echoUrl)
               .WithSegments("post")
               .AddFormFile(Generators.GenerateStream(), "file1.txt")
               .Post()
               .ReadJson<EchoResponse>();

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
               .WithCookies(new { k1 = "v1", k2 = "v2"})
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

        [Fact]
        public void Get_gets()
        {
            using var response = new Request(echoUrl).WithSegments("get").Get();

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public void Post_posts()
        {
            using var response = new Request(echoUrl).WithSegments("post").WithFormData(new { foo = "bar" }).Post();

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public void Put_puts()
        {
            using var response = new Request(echoUrl).WithSegments("put").WithFormData(new { foo = "bar" }).Put();

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public void Patch_patches()
        {
            using var response = new Request(echoUrl).WithSegments("patch").WithFormData(new { foo = "bar" }).Patch();

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public void Delete_deletes()
        {
            using var response = new Request(echoUrl).WithSegments("delete").WithFormData(new { foo = "bar" }).Delete();

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

    }
}
