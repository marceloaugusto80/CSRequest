using System.Net.Http;

namespace CSRequest.Internal
{
    public interface IRequestTransform
    {
        void Transform(HttpRequestMessage msg);
    }

}
