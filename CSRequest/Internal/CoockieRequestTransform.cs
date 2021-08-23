using System.Net.Http;
using System.Text;

namespace CSRequest.Internal
{
    public class CoockieRequestTransform : IRequestTransform
    {
        private readonly object cookies;

        public CoockieRequestTransform(object cookies)
        {
            this.cookies = cookies;
        }

        public void Transform(HttpRequestMessage msg)
        {
            if (cookies == null) return;

            var sb = new StringBuilder();
            foreach(var nv in cookies.ExtractPropertiesAndValues())
            {
                sb
                    .Append(nv.Name)
                    .Append('=')
                    .Append(nv.Value)
                    .Append(';');
            }
            sb.Remove(sb.Length - 1, 1);

            msg.Headers.Add("Cookie", sb.ToString());
        }
    }
}
