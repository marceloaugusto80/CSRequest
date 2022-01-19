using System.Net;

namespace CSRequest
{
    public class Request_Requests_Test
    {
        private readonly string echoUrl;

        public Request_Requests_Test()
        {
            echoUrl = @"http://postman-echo.com";
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
