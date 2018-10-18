using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ProbableEnigma
{
    internal class CacheService : ICacheService
    {
        private readonly ConcurrentDictionary<string, IStore> stores = new ConcurrentDictionary<string, IStore>();

        public IStore FindStore(string key) => stores.GetOrAdd(key, factory => new Store());
    }
}
