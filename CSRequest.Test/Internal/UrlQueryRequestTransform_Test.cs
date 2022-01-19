using System.Net.Http;

namespace CSRequest.Internal
{
    public class UrlQueryRequestTransform_Test
    {
        [Fact]
        public void UrlQueryRequestTransform_tests()
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, @"http://foobar.com");
            var query = new
            {
                foo = "bar",
                bar = "foo"
            };

            new UrlQueryRequestTransform(new ArgList(query)).Transform(request);

            request.RequestUri.AbsoluteUri.Should().Be("http://foobar.com/?foo=bar&bar=foo");
        }

    }
}