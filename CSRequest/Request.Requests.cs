using System.Net.Http;
using System.Threading.Tasks;

namespace CSRequest
{
    public partial class Request
    {
        /// <summary>
        /// Performs a get request.
        /// </summary>
        /// <returns>A <see cref="HttpResponseMessage"/>.</returns>
        public Task<HttpResponseMessage> GetAsync() => RequestAsync(HttpMethod.Get);

        /// <summary>
        /// Performs a post request.
        /// </summary>
        /// <returns>A <see cref="HttpResponseMessage"/>.</returns>
        public Task<HttpResponseMessage> PostAsync() => RequestAsync(HttpMethod.Post);

        /// <summary>
        /// Performs a put request.
        /// </summary>
        /// <returns>A <see cref="HttpResponseMessage"/>.</returns>
        public Task<HttpResponseMessage> PutAsync() => RequestAsync(HttpMethod.Put);

        /// <summary>
        /// Performs a patch request.
        /// </summary>
        /// <returns>A <see cref="HttpResponseMessage"/>.</returns>
        public Task<HttpResponseMessage> PatchAsync() => RequestAsync(HttpMethod.Patch);

        /// <summary>
        /// Performs a delete request.
        /// </summary>
        /// <returns>A <see cref="HttpResponseMessage"/>.</returns>
        public Task<HttpResponseMessage> DeleteAsync() => RequestAsync(HttpMethod.Delete);

        /// <summary>
        /// Performs a syncronous get request.
        /// </summary>
        /// <returns>A <see cref="HttpResponseMessage"/>.</returns>
        public HttpResponseMessage Get() => GetAsync().Result;

        /// <summary>
        /// Performs a syncronous post request.
        /// </summary>
        /// <returns>A <see cref="HttpResponseMessage"/>.</returns>
        public HttpResponseMessage Post() => PostAsync().Result;

        /// <summary>
        /// Performs a syncronous put request.
        /// </summary>
        /// <returns>A <see cref="HttpResponseMessage"/>.</returns>
        public HttpResponseMessage Put() => PutAsync().Result;

        /// <summary>
        /// Performs a syncronous patch request.
        /// </summary>
        /// <returns>A <see cref="HttpResponseMessage"/>.</returns>
        public HttpResponseMessage Patch() => PatchAsync().Result;

        /// <summary>
        /// Performs a syncronous delete request.
        /// </summary>
        /// <returns>A <see cref="HttpResponseMessage"/>.</returns>
        public HttpResponseMessage Delete() => DeleteAsync().Result;
    }
}
