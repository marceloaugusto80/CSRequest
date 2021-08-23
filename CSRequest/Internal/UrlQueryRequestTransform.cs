using System;
using System.Net.Http;
using System.Text;
using System.Web;

namespace CSRequest.Internal
{
    public class UrlQueryRequestTransform : IRequestTransform
    {
        private readonly object query;

        public UrlQueryRequestTransform(object query)
        {
            this.query = query;
        }

        public void Transform(HttpRequestMessage msg)
        {
            if (query == null) return;

            var sb = new StringBuilder(msg.RequestUri.AbsoluteUri, 256);
            sb.Append('?');
            foreach (var nv in query.ExtractPropertiesAndValues())
            {
                sb
                    .Append(HttpUtility.UrlEncode(nv.Name))
                    .Append('=')
                    .Append(HttpUtility.UrlEncode(nv.Value))
                    .Append('&');
            }

            sb.Remove(sb.Length - 1, 1);

            msg.RequestUri = new Uri(sb.ToString());

        }
    }
}
