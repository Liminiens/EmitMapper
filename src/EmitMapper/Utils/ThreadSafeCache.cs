using System;
using System.Collections.Concurrent;

namespace EmitMapper.Utils
{
    internal class ThreadSafeCache<TValue>
    {
        private readonly ConcurrentDictionary<string, Lazy<TValue>> _cache = new ConcurrentDictionary<string, Lazy<TValue>>();

        public TValue Get(string key, Func<TValue> getter)
        {
            return _cache.GetOrAdd(key, new Lazy<TValue>(getter)).Value;
        }
    }
}
