using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace ProbableEnigma
{
    public class ProbableEnigmaMiddleware
    {
        private readonly RequestDelegate _next;

        private string defaultStore = "default";
        private string defaultKey = "default";

        public ProbableEnigmaMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public static (string, string) ParsePath(string path, string defaultStore, string defaultKey)
        {
            string key = null;
            string store = null;
            var parts = path.Split("/");
            for (var i = parts.Length - 1; i >= 0; i--)
            {
                var part = parts[i];

                if (key == null) key = part.ToLowerInvariant();
                else if (store == null) store = part.ToLowerInvariant();
            }
            if (String.IsNullOrEmpty(store)) store = defaultStore.ToLowerInvariant();
            if (String.IsNullOrEmpty(key)) key = defaultKey.ToLowerInvariant();
            return (store, key);
        }

        public async Task Invoke(HttpContext context)
        {
            (string storeKey, string key) = ParsePath(context.Request.Path.Value, defaultStore, defaultKey);
            var method = context.Request.Method;

            var store = FindStore(storeKey);

            CacheItem storeItem = store.Get(key);
            CacheItem item = null;

            switch (method)
            {
                case "POST":
                    if (storeItem != null)
                    {
                        context.Response.StatusCode = StatusCodes.Status409Conflict;
                        await context.Response.WriteAsync($"{key} already exists!");
                        break;
                    }
                    item = await FromContext(context);
                    store.Put(key, item);
                    context.Response.StatusCode = StatusCodes.Status201Created;
                    break;
                case "GET":
                    if (storeItem == null)
                    {
                        context.Response.StatusCode = StatusCodes.Status404NotFound;
                        break;
                    }
                    context.Response.StatusCode = StatusCodes.Status200OK;
                    context.Response.ContentType = storeItem.ContentType;
                    context.Response.ContentLength = storeItem.Body.Length;
                    await context.Response.Body.WriteAsync(storeItem.Body, 0, storeItem.Body.Length);
                    break;
                case "PUT":
                    item = await FromContext(context);
                    store.Put(key, item);
                    context.Response.StatusCode = storeItem != null ? StatusCodes.Status202Accepted : StatusCodes.Status201Created;
                    break;
                case "DELETE":
                    if (storeItem == null)
                    {
                        context.Response.StatusCode = StatusCodes.Status204NoContent;
                    }
                    else
                    {
                        store.Put(key, null);
                        context.Response.StatusCode = StatusCodes.Status202Accepted;
                    }
                    break;
            }
        }

        public static async Task<byte[]> ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = await input.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    await ms.WriteAsync(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        private async Task<CacheItem> FromContext(HttpContext context)
        {
            return new CacheItem()
            {
                ContentType = context.Request.ContentType,
                Body = await ReadFully(context.Request.Body)
            };
        }

        private readonly ConcurrentDictionary<string, IStore> stores = new ConcurrentDictionary<string, IStore>();
        
        private IStore FindStore(string key) => stores.GetOrAdd(key, factory => new Store());
    }
}
