using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace CSRequest.Internal
{
    public class JsonContentRequestTransform : IRequestTransform
    {
        private readonly object data;

        public JsonContentRequestTransform(object data)
        {
            this.data = data;
        }

        public void Transform(HttpRequestMessage msg)
        {
            var json = JsonConvert.SerializeObject(data);
            msg.Content = new StringContent(json, Encoding.UTF8, "application/json");
        }
    }
}

namespace CSRequest
{
    using Internal;
    using System;

    public static class JsonContentExtension
    {
        /// <summary>
        /// Adds a json object in the request body. Sets the content-type to application/json.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="body">The object to be converted into a json object.</param>
        /// <remarks>Excludes any added form data and form file.</remarks>
        /// <returns>fluent.</returns>
        /// <exception cref="ArgumentNullException"/>
        public static Request WithJsonBody(this Request request, object body)
        {
            if (body == null) throw new ArgumentNullException(nameof(body));
            request.Transforms.Add(new JsonContentRequestTransform(body));
            return request;
        }
    }
}