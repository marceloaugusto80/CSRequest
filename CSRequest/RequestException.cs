using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSRequest
{
    /// <summary>
    /// Base exception for requests.
    /// </summary>
    [Serializable]
    public class RequestException : Exception
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public RequestException() { }
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="message">The message.</param>
        public RequestException(string message) : base(message) { }
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner exception.</param>
        public RequestException(string message, Exception inner) : base(message, inner) { }
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        protected RequestException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
