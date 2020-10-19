using CSRequest.Test.Helpers;
using FluentAssertions;
using Xunit;

namespace CSRequest.Test
{
    [Collection(nameof(ServerFixtureCollection))]
    public class Request_Test
    {
        [Fact]
        public void Get_returns_response()
        {
            var response = new Request()
                .WithSegments("get")
                .WithQuery(new { query1 = "querydata1", query2 = "querydata2" })
                .WithHeader(new { header1 = "headerdata1", header2 = "headerdata2" })
                .OnSuccess(resp => resp.Should().NotBeNull())
                .Get()
                .ReadJson<EchoResponse>();

            response.Args.Should().Contain("query1", "querydata1");
            response.Args.Should().Contain("query2", "querydata2");
            response.Headers.Should().Contain("header1", "headerdata1");
            response.Headers.Should().Contain("header2", "headerdata2");
        }

        [Fact]
        public void Post_returns_response()
        {
            var response = new Request()
                .WithSegments("post")
                .WithQuery(new { query1 = "querydata1", query2 = "querydata2" })
                .WithHeader(new { header1 = "headerdata1", header2 = "headerdata2" })
                .WithFormData(new { someString = "test string", someNumber = 42 })
                .AddFormFile(Generators.GenerateStream(), "file1.dat")
                .OnSuccess(resp => resp.Should().NotBeNull())
                .Post()
                .ReadJson<EchoResponse>();

            response.Args.Should().Contain("query1", "querydata1");
            response.Args.Should().Contain("query2", "querydata2");
            response.Headers.Should().Contain("header1", "headerdata1");
            response.Headers.Should().Contain("header2", "headerdata2");
            response.Form.Should().Contain("someString", "test string");
            response.Form.Should().Contain("someNumber", "42");
            response.Files.Should().ContainKey("file1.dat");
        }
    }
}
