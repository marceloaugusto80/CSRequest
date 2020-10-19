using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace CSRequest
{
    public static class ResponseExtensions
    {
        public static async Task<string> ReadStringAsync(this HttpResponseMessage msg)
        {
            try
            {
                var result = await msg.Content.ReadAsStringAsync().ConfigureAwait(false);
                return result;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                msg.Dispose();
            }
        }

        public static string ReadString(this HttpResponseMessage msg)
        {
            return msg.ReadStringAsync().Result;
        }

        public static async Task<T> ReadJsonAsync<T>(this HttpResponseMessage msg)
        {
            try
            {
                var json = await msg.ReadStringAsync().ConfigureAwait(false);
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                msg.Dispose();
            }
        }

        public static T ReadJson<T>(this HttpResponseMessage msg)
        {
            return msg.ReadJsonAsync<T>().Result;
        }

        public static async Task<dynamic> ReadJsonAsync(this HttpResponseMessage msg)
        {
            try
            {
                var json = await msg.ReadStringAsync().ConfigureAwait(false);
                return JsonConvert.DeserializeObject<dynamic>(json);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                msg.Dispose();
            }
        }

        public static dynamic ReadJson(this HttpResponseMessage msg)
        {
            return msg.ReadJsonAsync().Result;
        }

        public static async Task<Stream> ReadStreamAsync(this HttpResponseMessage msg)
        {
            try
            {
                var result = await msg.Content.ReadAsStreamAsync().ConfigureAwait(false);
                return result;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                msg.Dispose();
            }
        }

        public static Stream ReadStream(this HttpResponseMessage msg)
        {
            return msg.ReadStreamAsync().Result;
        }
    }
}
