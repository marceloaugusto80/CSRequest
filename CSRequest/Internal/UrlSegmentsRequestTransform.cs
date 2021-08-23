using System;
using System.Net.Http;
using System.Text;

namespace CSRequest.Internal
{
    public class UrlSegmentsRequestTransform : IRequestTransform
    {
        private readonly string[] segments;

        public UrlSegmentsRequestTransform(params string[] segments)
        {
            this.segments = segments;
        }

        public void Transform(HttpRequestMessage msg)
        {
            var url = msg.RequestUri.AbsoluteUri;
            var sb = new StringBuilder(url, 256);
            if(!url.EndsWith('/'))
            {
                sb.Append('/');
            }
            sb.AppendJoin('/', segments);
            msg.RequestUri = new Uri(sb.ToString());
        }
    }
}
