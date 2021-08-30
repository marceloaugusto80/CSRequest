using System;
using System.Net.Http;
using System.Text;
using System.Web;

namespace CSRequest.Internal
{
    public class UrlQueryRequestTransform : IRequestTransform
    {
        private readonly ArgList args;

        public UrlQueryRequestTransform(ArgList args)
        {
            this.args = args;
        }

        public void Transform(HttpRequestMessage msg)
        {
            var sb = new StringBuilder(msg.RequestUri.AbsoluteUri, 256);
            sb.Append('?');
            foreach (var nv in args.ToDictionary())
            {
                sb
                    .Append(HttpUtility.UrlEncode(nv.Key))
                    .Append('=')
                    .Append(HttpUtility.UrlEncode(nv.Value))
                    .Append('&');
            }

            sb.Remove(sb.Length - 1, 1);

            msg.RequestUri = new Uri(sb.ToString());
        }
    }
}

namespace CSRequest
{
    using Internal;
    using System;
    using System.Collections.Generic;

    public static class UrlQueryExtension
    {
        /// <summary>
        /// Appends a query string to the url. new { foo="bar", bar="foo"} will be evaluated to &lt;BASE_URL&gt;?foo=bar&amp;bar=foo.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="query">The object to be converted into a query string.</param>
        /// <returns>Fluent.</returns>
        /// <exception cref="ArgumentNullException"/>
        public static Request WithQuery(this Request request, object query)
        {
            if (query == null) throw new ArgumentNullException(nameof(query));
            request.Transforms.Add(new UrlQueryRequestTransform(new ArgList(query)));
            return request;
        }

        /// <summary>
        /// Appends a query string to the url.<br/>
        /// new Dictionary&lt;string, string&gt;(){{ foo="bar", bar="foo"}} will be evaluated to ?foo=bar&amp;bar=foo.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="query">The dictionary to be converted into a query string.</param>
        /// <returns>Fluent.</returns>
        /// <exception cref="ArgumentNullException"/>
        public static Request WithQuery(this Request request, Dictionary<string, object> query)
        {
            if (query == null) throw new ArgumentNullException(nameof(query));
            request.Transforms.Add(new UrlQueryRequestTransform(new ArgList(query)));
            return request;
        }

        /// <summary>
        /// Appends a query string to the url.<br/>
        /// [("foo", "bar"), ("bar", "foo")] will be evaluated to ?foo=bar&amp;bar=foo.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="query">The tuples params to be converted into a query string.</param>
        /// <returns>Fluent.</returns>
        /// <exception cref="ArgumentNullException"/>
        public static Request WithQuery(this Request request, params (string, string)[] query)
        {
            if (query == null) throw new ArgumentNullException(nameof(query));
            request.Transforms.Add(new UrlQueryRequestTransform(new ArgList(query)));
            return request;
        }
    }
}