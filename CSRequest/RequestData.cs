﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;

namespace CSRequest
{
    internal class RequestData
    {
        private readonly string baseUrl;
        public string BearerToken { get; set; }
        public object Query { get; set; }
        public object Header { get; set; }
        public object FormData { get; set; }
        public object JsonBody { get; set; }
        public object CookieObj { get; set; }
        public Dictionary<string, Stream> FormFiles { get; }
        public List<string> SegmentList { get; }

        public RequestData(string baseUrl = null)
        {
            this.FormFiles = new Dictionary<string, Stream>();
            this.SegmentList = new List<string>();
            this.baseUrl = baseUrl;
        }

        public HttpRequestMessage BuildRequest(HttpMethod method)
        {
            // TODO solve double slash issues
            string segments = SegmentList.ToUrlPath();
            string query = Query.ToQueryString();
            string fullUrl = $"{baseUrl}/{segments}{query}";
            
            var request = new HttpRequestMessage(method, fullUrl);

            // headers

            if (Header != null)
            {
                foreach (var kv in Header.ToHeaderDictionary())
                {
                    request.Headers.Add(kv.Key, kv.Value);
                }
            }

            if (!string.IsNullOrEmpty(BearerToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", BearerToken);
            }

            // body

            if (JsonBody != null)
            {
                request.Content = JsonBody.ToJsonContent();
            }
            else if (FormData != null || FormFiles.Count > 0)
            {
                var content = new MultipartFormDataContent();

                content.AddFormData(FormData);

                foreach (var file in FormFiles)
                    content.AddFormFile(file.Value, file.Key);

                request.Content = content;
            }

            // cookies

            if (CookieObj != null)
            {
                request.Headers.Add("Cookie", CookieObj.ToCookieString());
            }

            // 

            return request;
        }

    }

}
