using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate.Caches.Redis;
using StackExchange.Redis;

namespace Helper
{
    public class MyRedisCache : RedisCache
    {
        public const string SkipNHibernateCacheKey = "__SkipNHibernateCache__";

        public MyRedisCache(string regionName, IDictionary<string, string> properties, RedisCacheElement element, ConnectionMultiplexer connectionMultiplexer, RedisCacheProviderOptions options)
            : base(regionName, properties, element, connectionMultiplexer, options)
        {


        }

        public override object Get(object key)
        {
            if (HasFailedForWcfRequest())
            {
                return null;
            }
            else
            {
                return base.Get(key);
            }

        }

        public override void Put(object key, object value)
        {
            if (HasFailedForWcfRequest()) return;
            base.Put(key, value);
        }

        public override void Remove(object key)
        {
            if (HasFailedForWcfRequest()) return;
            base.Remove(key);
        }

        public override void Clear()
        {
            if (HasFailedForWcfRequest()) return;
            base.Clear();
        }

        public override void Destroy()
        {
            if (HasFailedForWcfRequest()) return;
            base.Destroy();
        }

        public override void Lock(object key)
        {
            if (HasFailedForWcfRequest()) return;
            base.Lock(key);
        }

        public override void Unlock(object key)
        {
            if (HasFailedForWcfRequest()) return;
            base.Unlock(key);
        }

        private bool HasFailedForWcfRequest()
        {
            try
            {
                return false;

            }
            catch
            {
                return true;
            }

        }
    }
}
