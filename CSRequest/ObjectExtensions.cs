using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Web;

namespace CSRequest
{
    internal static class ObjectExtensions
    {
        public static Dictionary<string, object> ToDictionary(this object obj)
        {
            if (obj == null) return null;

            var props = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            return props.ToDictionary(p => p.Name, p => p.GetValue(obj));
        }

        public static string ToQueryString(this object obj)
        {
            if (obj == null) return string.Empty;

            var dic = obj.ToDictionary();

            var parameters = dic.Select(
                kv => $"{HttpUtility.UrlEncode(kv.Key)}={HttpUtility.UrlEncode(Convert.ToString(kv.Value))}");

            return $"?{string.Join('&', parameters)}";
        }

        public static MultipartContent ToJsonContent(this object obj)
        {
            if (obj == null) return null;

            var content = new MultipartContent();

            var json = JsonConvert.SerializeObject(obj);
            var jsonContent = new StringContent(json);
            content.Add(jsonContent);

            return content;
        }

        public static void AddAsFormData(this MultipartFormDataContent form, object obj)
        {
            if (obj == null) return;

            foreach (var kv in obj.ToDictionary())
            {
                form.Add(
                    new StringContent(Convert.ToString(kv.Value), Encoding.UTF8),
                    kv.Key);
            }

        }

        public static void AddAsFormFile(this MultipartFormDataContent form, Stream stream, string fileName)
        {
            form.Add(new StreamContent(stream), fileName, fileName);
        }

        public static string ToCookieString(this object obj)
        {
            if (obj == null) return string.Empty;

            var dic = obj.ToDictionary();

            return string.Join(';', dic.Select(kv => $"{kv.Key}={Convert.ToString(kv.Value)}"));
        }

        public static string ToUrlPath(this IEnumerable<string> paths)
        {
            return string.Join('/', paths);
        }

    }
}
