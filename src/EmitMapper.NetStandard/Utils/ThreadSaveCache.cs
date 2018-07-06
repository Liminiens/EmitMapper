using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace EmitMapper.NetStandard.Utils
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
