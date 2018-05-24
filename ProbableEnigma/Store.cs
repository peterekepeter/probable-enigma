using System.Collections.Concurrent;
using System.Collections.Generic;

namespace ProbableEnigma
{
    internal class Store : IStore
    {
        private readonly ConcurrentDictionary<string, CacheItem> data = new ConcurrentDictionary<string, CacheItem>();

        public CacheItem Get(string key) => data.GetValueOrDefault(key);

        public void Put(string key, CacheItem item) => data.AddOrUpdate(key, item, (_, __) => item);
    }
}