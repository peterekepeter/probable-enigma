namespace ProbableEnigma
{
    internal interface IStore
    {
        /// <summary>
        /// Find and returns existing item.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        CacheItem Get(string key);

        /// <summary>
        /// Updates existing item.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        void Put(string key, CacheItem item);
    }
}