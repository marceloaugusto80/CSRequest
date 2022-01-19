using System.Collections.Concurrent;
using System.Reflection;

namespace CSRequest.Internal
{
    public class PropertyInfoCache
    {
        private readonly ConcurrentDictionary<string, PropertyInfo[]> typePropMap;

        public int Size { get => typePropMap.Count; }

        public PropertyInfoCache()
        {
            typePropMap = new ConcurrentDictionary<string, PropertyInfo[]>();
        }

        public PropertyInfo[] GetProperties(object obj)
        {
            string typeName = obj.GetType().Name;
            return typePropMap.GetOrAdd(typeName, (objType) =>
            {
                return obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            });
        }

    }
}
