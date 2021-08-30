using System;
using System.Collections.Generic;
using System.Net.Http;

namespace CSRequest.Internal
{

    public class HeaderRequestTransform : IRequestTransform
    {
        private readonly ArgList args;

        public HeaderRequestTransform(ArgList args)
        {
            this.args = args;
        }

        public void Transform(HttpRequestMessage msg)
        {
            if(args != null)
            {
                foreach (var kv in args.ToDictionary())
                {
                    msg.Headers.Add(kv.Key, kv.Value.Replace('_', '-'));
                }
            }
        }
    }

}

namespace CSRequest
{
    using CSRequest.Internal;
    using System.Text;
    
    public static class HeaderExtensions
    {
        /// <summary>
        /// Add headers to the request. new { foo = "bar" } will be evaluated to foo: bar.<br/>
        /// Underscores (_) are translated to dashes (-), like: new { Content_type = "plain/text" } will be evaluated to Content-type: plain/text.
        /// </summary>
        /// <param name="request">Extension.</param>
        /// <param name="header">An object that represents the headers.</param>
        /// <returns>Fluent.</returns>
        /// <exception cref="ArgumentNullException"/>
        public static Request WithHeader(this Request request, object header)
        {
            if (header == null) throw new ArgumentNullException(nameof(header));
            request.Transforms.Add(new HeaderRequestTransform(new ArgList(header)));
            return request;
        }

        /// <summary>
        /// Add headers to the request.<br/>
        /// new Dictionary&lt;string, object&gt;(){{ "foo", "bar" }} will be evaluated to foo: bar.<br/>
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="header">A dictionary that represent the headers. Keys are the header names, values are the header values.</param>
        /// <returns>Fluent.</returns>
        /// <exception cref="ArgumentNullException"/>
        public static Request WithHeader(this Request request, Dictionary<string, object> header)
        {
            if (header == null) throw new ArgumentNullException(nameof(header));
            request.Transforms.Add(new HeaderRequestTransform(new ArgList(header)));
            return request;
        }

        /// <summary>
        /// Add headers to the request.<br/>
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="header">Tuples params that represents the header names (Item1) and header values (Item2).</param>
        /// <returns>Fluent.</returns>
        /// <exception cref="ArgumentNullException"/>
        public static Request WithHeader(this Request request, params (string, string)[] header)
        {
            if (header == null) throw new ArgumentNullException(nameof(header));
            request.Transforms.Add(new HeaderRequestTransform(new ArgList(header)));
            return request;
        }

        /// <summary>
        /// Adds a OAuth bearer token authorization header in the format 'Authorization: Bearer &lt;TOKEN&gt;'
        /// </summary>
        /// <param name="request"></param>
        /// <param name="token">The bearer token. This method doesn't encode the token.</param>
        /// <returns>Fluent.</returns>
        /// <exception cref="ArgumentNullException"/>
        public static Request WithBearerTokenAuthentication(this Request request, string token)
        {
            if (token == null) throw new ArgumentNullException(nameof(token));
            request.Transforms.Add(
                new HeaderRequestTransform(
                    new ArgList(("Authorization", $"Bearer {token}"))));
            return request;
        }

        /// <summary>
        /// Sends request with basic authentication header.<br/>
        /// Adds the header 'Authorization: Basic &lt;BASE64_CREDENTIALS&gt;'.<br/>
        /// Username and password are converted to base64.<br/>
        /// See: https://developer.mozilla.org/en-US/docs/Web/HTTP/Authentication#basic_authentication_scheme
        /// </summary>
        /// <param name="request"></param>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <returns>Fluent</returns>
        /// <exception cref="ArgumentException"/>
        public static Request WithBasicAuthentication(this Request request, string username, string password)
        {
            if (string.IsNullOrEmpty(username))
                throw new ArgumentException($"{nameof(username)} is null or empty");

            if (string.IsNullOrEmpty(password))
                throw new ArgumentException($"{nameof(password)} is null or empty");

            var base64Credentials = Convert.ToBase64String(
                Encoding.ASCII.GetBytes($"{username}:{password}"));

            request.Transforms.Add(
                new HeaderRequestTransform(
                    new ArgList(("Authorization", $"Basic {base64Credentials}"))));
            return request;
        }

        /// <summary>
        /// Adds a authentication header in the form 'Authorization: &lt;SCHEME&gt; &lt;CREDENTIALS&gt;'<br/>
        /// Credentials are NOT encoded in any way.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="scheme">The autorization Scheme.</param>
        /// <param name="credentials">The credentials</param>
        /// <returns>Fluent</returns>
        public static Request WithAuthorization(this Request request, string scheme, string credentials)
        {
            if (string.IsNullOrEmpty(scheme))
                throw new ArgumentException($"{nameof(scheme)} is null or empty");

            if (string.IsNullOrEmpty(credentials))
                throw new ArgumentException($"{nameof(credentials)} is null or empty");

            request.Transforms.Add(
                new HeaderRequestTransform(
                    new ArgList(("Authorization", $"{scheme} {credentials}"))));
            return request;
        }
    }

}
