using System.Net.Http;

namespace CSRequest.Internal
{
    public class FormDataRequestTransform : IRequestTransform
    {
        private readonly ArgList args;

        public FormDataRequestTransform(ArgList args)
        {
            this.args = args;
        }

        public void Transform(HttpRequestMessage msg)
        {
            //check if content was already set by FormFileRequestTransform
            var content = msg.Content as MultipartFormDataContent ?? new MultipartFormDataContent();
            
            foreach (var nv in args.ToDictionary())
            {
                content.Add(new StringContent(nv.Value), nv.Key);
            }

            msg.Content = content;
            
        }
    }
}

namespace CSRequest
{
    using Internal;
    using System;
    using System.Collections.Generic;

    public static class FormDataExtension
    {
        /// <summary>
        /// Adds form data to the request body. The request content-type is set to multipart/form-data.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="formData">The object to be converted into form data.</param>
        /// <returns>Fluent.</returns>
        /// <exception cref="ArgumentNullException"/>
        public static Request WithFormData(this Request request, object formData)
        {
            if (formData == null) throw new ArgumentNullException(nameof(formData));
            request.Transforms.Add(new FormDataRequestTransform(new ArgList(formData)));
            return request;
        }

        /// <summary>
        /// Adds form data. The request content-type is set to multipart/form-data.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="formData">The Dictionary to be converted into form data.</param>
        /// <returns>Fluent.</returns>
        /// <exception cref="ArgumentNullException"/>
        public static Request WithFormData(this Request request, Dictionary<string, object> formData)
        {
            if (formData == null) throw new ArgumentNullException(nameof(formData));
            request.Transforms.Add(new FormDataRequestTransform(new ArgList(formData)));
            return request;
        }

        /// <summary>
        /// Adds form data. The request content-type is set to multipart/form-data.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="formData">The tuples to be converted into form data.</param>
        /// <returns>Fluent.</returns>
        /// <exception cref="ArgumentNullException"/>
        public static Request WithFormData(this Request request, params (string, string)[] formData)
        {
            if (formData == null) throw new ArgumentNullException(nameof(formData));
            request.Transforms.Add(new FormDataRequestTransform(new ArgList(formData)));
            return request;
        }
    }
}
