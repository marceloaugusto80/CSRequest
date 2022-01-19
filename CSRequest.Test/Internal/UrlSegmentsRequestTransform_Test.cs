using System.Net.Http;

namespace CSRequest.Internal
{
    public class UrlSegmentsRequestTransform_Test
    {
        [Fact]
        public void UrlSegmentsRequestTransform_tests()
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, @"http://foobar.com");

            new UrlSegmentsRequestTransform("foo", "bar").Transform(request);

            request.RequestUri.AbsoluteUri.Should().Be("http://foobar.com/foo/bar");
        }
    }
}