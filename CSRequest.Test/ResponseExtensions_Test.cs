using CSRequest.Test.Helpers;
using FluentAssertions;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace CSRequest.Test
{
    [Collection(nameof(ServerFixtureCollection))]
    public class ResponseExtensions_Test
    {
        [Fact]
        public async Task ReadJsonAsync_reads_json_response()
        {
            using var response = await new Request().WithSegments("get").GetAsync();

            var actual = await response.ReadJsonAsync<EchoResponse>();

            actual.Should().NotBeNull();
        }

        [Fact]
        public async Task ReadJsonAsync_reads_json_response_as_dynamic_object()
        {
            using var response = await new Request().WithSegments("get").GetAsync();

            var actual = await response.ReadJsonAsync();

            Assert.NotNull(actual);
        }

        [Fact]
        public async Task ReadStringAsync_reads_string_response()
        {
            using var response = await new Request()
                .InjectClient(() => 
                    new HttpClient() { BaseAddress = new Uri(@"https://baconipsum.com/") })
                .WithSegments("api")
                .WithQuery(new { type = "meat-and-filler", format = "text"})
                .GetAsync();

            var actual = await response.ReadStringAsync();

            actual.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task ReadStreamAsync_reads_stream_response()
        {
            using var response = await new Request()
                .WithSegments("stream", "1")
                .GetAsync();

            var actual = await response.ReadStreamAsync();

            actual.Length.Should().BePositive();
        }
    }
}
