using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NHibernate.Caches.Redis;
using StackExchange.Redis;

namespace Helper
{
    public class MyRedisCacheProvider : RedisCacheProvider
    {

        public MyRedisCacheProvider()
        {
        }

        protected override RedisCache BuildCache(string regionName, IDictionary<string, string> properties, RedisCacheElement configElement, ConnectionMultiplexer connectionMultiplexer, RedisCacheProviderOptions options)
        {
          

            var cache = new MyRedisCache(regionName, properties, configElement, connectionMultiplexer, options);

            return cache;
        }

    }
}
