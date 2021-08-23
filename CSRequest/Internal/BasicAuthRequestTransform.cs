using System;
using System.Net.Http;
using System.Text;

namespace CSRequest.Internal
{
    public class BasicAuthRequestTransform : IRequestTransform
    {
        private readonly string username;
        private readonly string password;

        public BasicAuthRequestTransform(string username, string password)
        {
            this.username = username;
            this.password = password;
        }

        public void Transform(HttpRequestMessage msg)
        {
            var base64Data = Convert.ToBase64String(
                Encoding.ASCII.GetBytes($"{username}:{password}"));
            msg.Headers.Add("Authorization", $"Basic {base64Data}");
        }
    }
}
