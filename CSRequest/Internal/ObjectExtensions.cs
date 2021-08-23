using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CSRequest.Internal
{
    internal static class ObjectExtensions
    {
        public static IEnumerable<(string Name, string Value)> ExtractPropertiesAndValues(this object obj)
        {
            return obj
                .GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Select(p => (p.Name.Replace('_', '-'), p.GetValue(obj).ToString()));
        }
    }
}
