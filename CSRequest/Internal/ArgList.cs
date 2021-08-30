﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CSRequest.Internal
{
    public class ArgList
    {
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
            return obj
                .GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .ToDictionary(
                    p => keyTransform  == null ? p.Name : keyTransform.Invoke(p.Name), 
                    p => Convert.ToString(p.GetValue(obj).ToString()));
        }
    }
}
