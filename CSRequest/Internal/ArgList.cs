using System;
using System.Collections.Generic;
using System.Linq;

namespace CSRequest.Internal
{
    public sealed class ArgList
    {
        private static readonly PropertyInfoCache propCache;
        static ArgList()
        {
            propCache = new PropertyInfoCache();
        }

        private readonly Dictionary<string, string> argsDictionary;

        public ArgList(params (string, string)[] keyValues)
        {
            this.argsDictionary = keyValues.ToDictionary(kv => kv.Item1, kv => kv.Item2);
        }

        public ArgList(object obj, Func<string, string> keyTransform = null)
        {
            this.argsDictionary = ConvertObjectToDictionary(obj, keyTransform);
        }

        public ArgList(Dictionary<string, object> keyValues)
        {
            this.argsDictionary = keyValues.ToDictionary(kv => kv.Key, kv => Convert.ToString(kv.Value));
        }

        public Dictionary<string, string> ToDictionary()
        {
            return argsDictionary;
        }

        private static Dictionary<string, string> ConvertObjectToDictionary(object obj, Func<string, string> keyTransform = null)
        {
            return propCache.GetProperties(obj)
                .ToDictionary(
                    p => keyTransform?.Invoke(p.Name) ?? p.Name,
                    p => Convert.ToString(p.GetValue(obj)));
        }
    }
}
