using System.Net.Http;


namespace CSRequest.Internal
{
    public class JsonContentRequestTransform_Test
    {

        [Fact]
        public async Task JsonContentRequestTransform_tests()
        {
            using var request = new HttpRequestMessage();
            var obj = new { foo = "bar", bar = "foo" };

            new JsonContentRequestTransform(obj).Transform(request);

            request.Content.Headers.ContentType.MediaType.Should().Be("application/json");
            (await request.Content.ReadAsStringAsync()).Should().Be(@"{""foo"":""bar"",""bar"":""foo""}");
        }
    }
}