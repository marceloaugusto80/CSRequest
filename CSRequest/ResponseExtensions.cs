using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace CSRequest
{
    /// <summary>
    /// Extension methods for responses.
    /// </summary>
    public static class ResponseExtensions
    {
        /// <summary>
        /// Reads the response body as string. Disposes the <see cref="HttpResponseMessage"/>.
        /// </summary>
        /// <param name="msg">The response.</param>
        /// <returns>The response body as string.</returns>
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

        /// <summary>
        /// Reads the json response as a <typeparamref name="T"/> object. Disposes the <see cref="HttpResponseMessage"/>.
        /// </summary>
        /// <typeparam name="T">The object type that the json response will be converted.</typeparam>
        /// <param name="msg">The response.</param>
        /// <returns>A <typeparamref name="T"/> object.</returns>
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

        /// <summary>
        /// Reads the json response as a dynamic object. Disposes the <see cref="HttpResponseMessage"/>.
        /// </summary>
        /// <param name="msg">The response.</param>
        /// <returns>A dynamic object.</returns>
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

        /// <summary>
        /// Reads the stream content of the <see cref="HttpResponseMessage"/>. Disposes the <see cref="HttpResponseMessage"/>.
        /// </summary>
        /// <param name="msg">The response.</param>
        /// <returns>A <see cref="Stream"/>.</returns>
        public static async Task<Stream> ReadStreamAsync(this HttpResponseMessage msg)
        {
            try
            {
                var memory = new MemoryStream();
                using var responseStream = await msg.Content.ReadAsStreamAsync().ConfigureAwait(false);
                await responseStream.CopyToAsync(memory);
                if (memory.CanSeek) memory.Seek(0, SeekOrigin.Begin);
                return memory;
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

        /// <summary>
        /// Synchronously reads the response body as string. Disposes the <see cref="HttpResponseMessage"/>.
        /// </summary>
        /// <param name="msg">The response.</param>
        /// <returns>The response body as string.</returns>
        public static string ReadString(this HttpResponseMessage msg)
        {
            return msg.ReadStringAsync().Result;
        }

        /// <summary>
        /// Synchronously reads the json response as a <typeparamref name="T"/> object. Disposes the <see cref="HttpResponseMessage"/>.
        /// </summary>
        /// <typeparam name="T">The object type that the json response will be converted.</typeparam>
        /// <param name="msg">The response.</param>
        /// <returns>A <typeparamref name="T"/> object.</returns>
        public static T ReadJson<T>(this HttpResponseMessage msg)
        {
            return msg.ReadJsonAsync<T>().Result;
        }

        /// <summary>
        /// Synchronously reads the json response as a dynamic object. Disposes the <see cref="HttpResponseMessage"/>.
        /// </summary>
        /// <param name="msg">The response.</param>
        /// <returns>A dynamic object.</returns>
        public static dynamic ReadJson(this HttpResponseMessage msg)
        {
            return msg.ReadJsonAsync().Result;
        }

        /// <summary>
        /// Synchronously reads the stream content of the <see cref="HttpResponseMessage"/>. Disposes the <see cref="HttpResponseMessage"/>.
        /// </summary>
        /// <param name="msg">The response.</param>
        /// <returns>A <see cref="Stream"/>.</returns>
        public static Stream ReadStream(this HttpResponseMessage msg)
        {
            return msg.ReadStreamAsync().Result;
        }
    }
}
