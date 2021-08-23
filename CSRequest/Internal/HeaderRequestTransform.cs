﻿using System.Net.Http;

namespace CSRequest.Internal
{

    public class HeaderRequestTransform : IRequestTransform
    {
        private readonly object header;

        public HeaderRequestTransform(object header)
        {
            this.header = header;
        }

        public void Transform(HttpRequestMessage msg)
        {
            if (header == null) return;

            foreach (var kv in header.ExtractPropertiesAndValues())
            {
                msg.Headers.Add(kv.Name.Replace('_', '-'), kv.Value);
            }
        }
    }
}
