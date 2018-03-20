using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exf.Persistance.NHibernate;
using NHibernate.Caches.Redis;
using StackExchange.Redis;

namespace Helper
{
    public class NHibernateCacheFactory 
    {
        private static ConfigurationOptions _configurationOptions;

        private static readonly Lazy<ConnectionMultiplexer> LazyConnection = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(_configurationOptions));

        public static ConnectionMultiplexer ConnectionMultiplexer => LazyConnection.Value;

        public void SetupRedisCacheProvider(
            bool allowAdmin = false,
            bool abortOnConnectFail = false,
            int connectRetry = 1,
            string clientName = "",
            int connectTimeout = 100,
            string host = "localhost:6379",
            int portNumber = 6379,
            int databaseId = 0,
            int responseTimeout = 100,
            string serviceName = "",
            int keepAlive = 180)
        {

            string redisSecondLevelCacheHost;
            int redisSecondLevelCachePort;

            _configurationOptions = new ConfigurationOptions()
            {
                AllowAdmin = allowAdmin,
                AbortOnConnectFail = abortOnConnectFail,
                ConnectRetry = connectRetry,
                ClientName = clientName,
                ConnectTimeout = connectTimeout,
                DefaultDatabase = 0,
                ResponseTimeout = responseTimeout,
                ServiceName = serviceName,
                KeepAlive = keepAlive,

            };

            foreach (string item in host.Split(','))
            {
                redisSecondLevelCacheHost = item.Split(':')[0];
                redisSecondLevelCachePort = Convert.ToInt16(item.Split(':')[1]);
                _configurationOptions.EndPoints.Add(redisSecondLevelCacheHost, redisSecondLevelCachePort);
            }

            //https://stackexchange.github.io/StackExchange.Redis/Configuration
            var connectionMultiplexer = ConnectionMultiplexer;

            MyRedisCacheProvider.SetConnectionMultiplexer(connectionMultiplexer);

            MyRedisCacheProvider.SetOptions(new RedisCacheProviderOptions()
            {
                Serializer = new NhJsonCacheSerializer(),
                Database = databaseId,
            });
        }
    }
}

