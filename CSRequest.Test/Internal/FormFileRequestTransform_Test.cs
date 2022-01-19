using CSRequest.Test.Helpers;
using System.Net.Http;


namespace CSRequest.Internal
{
    public class FormFileRequestTransform_Test
    {

        [Fact]
        public void FormFileRequestTransform_tests()
        {
            using var request = new HttpRequestMessage();
            using var file = Generators.GenerateStream();

            new FormFileRequestTransform(file, "field", "file").Transform(request);

            request.Content.Headers.GetValues("Content-Type").First().Should().Contain("multipart/form-data;");
        }
    }
}