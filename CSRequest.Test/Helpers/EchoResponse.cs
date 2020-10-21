using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace CSRequest.Test.Helpers
{
    class EchoResponse
    {
        public string Url { get; set; }
        public Dictionary<string, string> Args { get; set; }
        public Dictionary<string, string> Headers { get; set; }
        public Dictionary<string, string> Form { get; set; }
        public Dictionary<string, string> Files { get; set; }
        public Dictionary<string, string> Cookies { get; set; }
        public JObject Json { get; set; }
    }
}
