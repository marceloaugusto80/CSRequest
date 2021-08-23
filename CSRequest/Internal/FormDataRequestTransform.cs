using System.Net.Http;

namespace CSRequest.Internal
{
    public class FormDataRequestTransform : IRequestTransform
    {
        private readonly object formData;

        public FormDataRequestTransform(object formData)
        {
            this.formData = formData;
        }

        public void Transform(HttpRequestMessage msg)
        {
            if (formData == null) return;

            var content = msg.Content as MultipartFormDataContent ?? new MultipartFormDataContent();
            foreach (var nv in formData.ExtractPropertiesAndValues())
            {
                content.Add(new StringContent(nv.Value), nv.Name);
            }

            msg.Content = content;
            
        }
    }
}
