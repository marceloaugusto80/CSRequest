using System.Net.Http;

namespace CSRequest.Internal
{
    public class CoockieRequestTransform_Test
    {
        [Fact]
        public void CookieRequestTransform_tests()
        {
            using var request = new HttpRequestMessage();
            var cookie = new
            {
                key1 = "value1",
                key2 = "value2"
            };

            new CoockieRequestTransform(new ArgList(cookie)).Transform(request);

            request.Headers.GetValues("Cookie").First().Should().Be("key1=value1;key2=value2");
        }

    }
}