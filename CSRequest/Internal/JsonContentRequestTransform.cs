using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace CSRequest.Internal
{
    public class JsonContentRequestTransform : IRequestTransform
    {
        private readonly object data;

        public JsonContentRequestTransform(object data)
        {
            this.data = data;
        }

        public void Transform(HttpRequestMessage msg)
        {
            var json = JsonConvert.SerializeObject(data);
            msg.Content = new StringContent(json, Encoding.UTF8, "application/json");
        }
    }
}
