using System.Net.Http;
using System.Text;

namespace CSRequest.Internal
{
    public class CoockieRequestTransform : IRequestTransform
    {
        private readonly ArgList args;

        public CoockieRequestTransform(ArgList args)
        {
            this.args = args;
        }

        public void Transform(HttpRequestMessage msg)
        {
            var sb = new StringBuilder();
            foreach (var nv in args.ToDictionary())
            {
                sb
                    .Append(nv.Key)
                    .Append('=')
                    .Append(nv.Value)
                    .Append(';');
            }
            sb.Remove(sb.Length - 1, 1);

            msg.Headers.Add("Cookie", sb.ToString());
        }
    }
}

namespace CSRequest
{
    using Internal;
    using System;
    using System.Collections.Generic;

    public static class CookieExtension
    {
        /// <summary>
        /// Set the request cookies.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cookies">An object that represents the key/values of the cookies.</param>
        /// <returns>Fluent.</returns>
        /// <exception cref="ArgumentNullException"/>
        public static Request WithCookies(this Request request, object cookies)
        {
            if (cookies == null) throw new ArgumentNullException(nameof(cookies));
            request.Transforms.Add(new CoockieRequestTransform(new ArgList(cookies)));
            return request;
        }

        /// <summary>
        /// Set the request cookies.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cookies">A Dictionary that represents the key/values of the cookies.</param>
        /// <returns>Fluent.</returns>
        /// <exception cref="ArgumentNullException"/>
        public static Request WithCookies(this Request request, Dictionary<string, object> cookies)
        {
            if (cookies == null) throw new ArgumentNullException(nameof(cookies));
            request.Transforms.Add(new CoockieRequestTransform(new ArgList(cookies)));
            return request;
        }

        /// <summary>
        /// Set the request cookies.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cookies">Tuples that represent the key/values of the cookies.</param>
        /// <returns>Fluent.</returns>
        /// <exception cref="ArgumentNullException"/>
        public static Request WithCookies(this Request request, params (string, string)[] cookies)
        {
            if (cookies == null) throw new ArgumentNullException(nameof(cookies));
            request.Transforms.Add(new CoockieRequestTransform(new ArgList(cookies)));
            return request;
        }

    }
}
