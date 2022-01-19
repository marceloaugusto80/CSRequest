using System.Net.Http;


namespace CSRequest.Internal
{
    public class FormDataRequestTransform_Test
    {
        [Fact]
        public void FormDataRequestTransform_tests()
        {
            using var request = new HttpRequestMessage();
            var formData = new
            {
                key1 = "value1",
                key2 = "value2"
            };

            new FormDataRequestTransform(new ArgList(formData)).Transform(request);

            request.Content.Headers.GetValues("Content-Type").First().Should().Contain("multipart/form-data;");
        }
    }
}