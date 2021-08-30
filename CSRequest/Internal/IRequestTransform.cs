using System.Net.Http;

namespace CSRequest.Internal
{
    /// <summary>
    /// Common interface for changing a <see cref="HttpRequestMessage"/> instances.
    /// </summary>
    public interface IRequestTransform
    {
        /// <summary>
        /// Mutates a <see cref="HttpRequestMessage"/> instance.
        /// </summary>
        /// <param name="msg">The message to mutate.</param>
        void Transform(HttpRequestMessage msg);
    }

}
