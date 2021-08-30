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

namespace CSRequest
{
    using Internal;
    using System;

    public static class UrlSegmentsExtension
    {
        /// <summary>
        /// Adds segments to the url. ["a", "b", "c"] will be evaluated as &lt;BASE_URL&gt;/a/b/c.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="segments">The segments to add to the request url.</param>
        /// <returns>Fluent</returns>
        /// <exception cref="ArgumentNullException"/>
        public static Request WithSegments(this Request request, params string[] segments)
        {
            if (segments == null) throw new ArgumentNullException(nameof(segments));
            request.Transforms.Add(new UrlSegmentsRequestTransform(segments));
            return request;
        }
    }
}
