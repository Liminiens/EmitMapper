using System;
using System.Collections.Concurrent;

namespace EmitMapper.Utils
{
    class ThreadSaveCache
    {
        private readonly ConcurrentDictionary<string, object> _cache = new ConcurrentDictionary<string, object>();

        public T Get<T>(string key, Func<object> getter)
        {
            if (!_cache.TryGetValue(key, out var value))
            {
                value = getter();
                _cache.TryAdd(key, value);
            }
            return (T)value;
        }
    }
}
