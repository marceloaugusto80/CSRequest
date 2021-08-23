using System.IO;
using System.Net.Http;

namespace CSRequest.Internal
{
    public class FormFileRequestTransform : IRequestTransform
    {
        private readonly Stream file;
        private readonly string fieldName;
        private readonly string fileName;

        public FormFileRequestTransform(Stream file, string fieldName, string fileName)
        {
            this.file = file;
            this.fieldName = fieldName;
            this.fileName = fileName;
        }

        public void Transform(HttpRequestMessage msg)
        {
            // TODO multipartcontent vs multipartformdatacontent
            var content = msg.Content as MultipartFormDataContent ?? new MultipartFormDataContent();
            content.Add(new StreamContent(file), fieldName, fileName);
            msg.Content = content;
        }
    }
}
