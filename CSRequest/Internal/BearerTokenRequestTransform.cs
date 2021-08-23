using System.Net.Http;

namespace CSRequest.Internal
{
    public class BearerTokenRequestTransform : IRequestTransform
    {
        private readonly string token;

        public BearerTokenRequestTransform(string token)
        {
            this.token = token;
        }

        public void Transform(HttpRequestMessage msg)
        {
            msg.Headers.Add("Authorization", $"Bearer {token}");
        }
    }
}
