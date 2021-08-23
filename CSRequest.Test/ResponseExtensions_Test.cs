using CSRequest.Test.Helpers;
using FluentAssertions;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace CSRequest.Test
{
    public class ResponseExtensions_Test
    {
        private readonly string echoUrl;
        private readonly string ipsumUrl;

        public ResponseExtensions_Test()
        {
            this.echoUrl = @"http://postman-echo.com";
            this.ipsumUrl = @"https://baconipsum.com";
        }

        [Fact]
        public async Task ReadJsonAsync_reads_json_response()
        {
            using var response = await new Request(echoUrl).WithSegments("get").GetAsync();

            var actual = await response.ReadJsonAsync<EchoResponse>();

            actual.Should().NotBeNull();
        }

        [Fact]
        public async Task ReadJsonAsync_reads_json_response_as_dynamic_object()
        {
            using var response = await new Request(echoUrl).WithSegments("get").GetAsync();

            var actual = await response.ReadJsonAsync();
            
            (actual.url as string).Should().Be(echoUrl + "/get");
        }

        [Fact]
        public async Task ReadStringAsync_reads_string_response()
        {
            using var response = await new Request(ipsumUrl)
                .WithSegments("api")
                .WithQuery(new { type = "meat-and-filler", format = "text"})
                .GetAsync();

            var actual = await response.ReadStringAsync();

            actual.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task ReadStreamAsync_reads_stream_response()
        {
            using var response = await new Request(echoUrl)
                .WithSegments("stream", "1")
                .GetAsync();

            var actual = await response.ReadStreamAsync();

            actual.Position.Should().Be(0);
            actual.Length.Should().BePositive();
        }
    }
}
