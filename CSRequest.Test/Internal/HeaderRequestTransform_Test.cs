using System.Net.Http;

namespace CSRequest.Internal
{
    public class HeaderRequestTransform_Test
    {

        [Fact]
        public void HeaderRequestTransform_tests()
        {
            using var request = new HttpRequestMessage();
            var header = new
            {
                foo = "bar",
                bar = "foo"
            };

            new HeaderRequestTransform(new ArgList(header)).Transform(request);

            request.Headers.GetValues("foo").First().Should().Be("bar");
            request.Headers.GetValues("bar").First().Should().Be("foo");
        }

    }
}