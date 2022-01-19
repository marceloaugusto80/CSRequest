using System.IO;
using System.Net.Http;

namespace CSRequest.Internal
{
    public class FormFileRequestTransform : IRequestTransform
    {
        private readonly Stream file;
        private readonly string fieldName;
        private readonly string fileName;

        public FormFileRequestTransform(Stream file, string fieldName, string fileName)
        {
            this.file = file;
            this.fieldName = fieldName;
            this.fileName = fileName;
        }

        public void Transform(HttpRequestMessage msg)
        {
            // Check if the content was already set by FormDataRequestTransform
            var content = msg.Content as MultipartFormDataContent ?? new MultipartFormDataContent();
            content.Add(new StreamContent(file), fieldName, fileName);
            msg.Content = content;
        }
    }
}

namespace CSRequest
{
    using Internal;
    using System;
    using System.Collections.Generic;

    public static class FormFileExtension
    {

        /// <summary>
        /// Adds a stream to be uploaded. Works like a html's file input element. The request content-type is set to multipart/form-data.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="stream">The data stream to be uploaded.</param>
        /// <param name="fieldName">The name of the form data field related to the file. If none is provided, a random field name will be created.</param>
        /// <param name="fileName">A file name attached to the stream. If none is provided, a random name will be created.</param>
        /// <returns>Fluent.</returns>
        /// <exception cref="ArgumentNullException"/>
        public static Request AddFormFile(this Request request, Stream stream, string fieldName = null, string fileName = null)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            var name = fileName ?? Path.GetRandomFileName();
            var field = fieldName ?? name;
            request.Transforms.Add(new FormFileRequestTransform(stream, field, name));
            return request;
        }

        /// <summary>
        /// Adds multiple streams to be uploaded. Works like a html's file input element. The request content-type is set to multipart/form-data.<br/>
        /// File names and field names associated with the files will be random.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="streams">The data streams to be uploaded. Random file names will be attached to each stream.</param>
        /// <returns>Fluent.</returns>
        /// <exception cref="ArgumentNullException"/>
        public static Request AddFormFile(this Request request, IEnumerable<Stream> streams)
        {
            if (streams == null) throw new ArgumentNullException(nameof(streams));
            int counter = 0;
            foreach (var stream in streams)
            {
                request.AddFormFile(stream, $"file{counter++}");
            }
            return request;
        }
    }

}