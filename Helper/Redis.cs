using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Helper
{
    public class Redis
    {
        private long startTime = 666;
        private long endTime = 0;

        private readonly ConfigurationOptions _configurationOptions;
        private IDatabase _db;
        private ConnectionMultiplexer _redisConnection;

        public Redis()
        {
            _configurationOptions = new ConfigurationOptions();
            _configurationOptions.EndPoints.Add("localhost");
            _configurationOptions.ConnectRetry = 1;


            _configurationOptions.ConnectTimeout = 500;


        }

        public void ValidateConfigValues()
        {
            
        }

        public string GetValue(int storeId, string key, string hashKey)
        {

            var retrievedValue = GetDb(storeId).HashGet(key, hashKey);


            return retrievedValue;
        }

        public string[] GetValues(int storeId, string key, params string[] hashKeys)
        {
            var values = Array.ConvertAll(hashKeys, x => (RedisValue)x);
            var redisValues = GetDb(storeId).HashGet(key, values);
            return redisValues.ToStringArray();
        }

        public void SetKey(int storeId, string key, string hashKey, string hashValue)
        {
            GetDb(storeId).HashSet(key, hashKey, hashValue);
        }

        public void DeleteKey(int storeId, string key)
        {
            GetDb(storeId).KeyDelete(key);
        }

        public bool KeyExists(int storeId, string key)
        {
            return GetDb(storeId).KeyExists(key);
        }

        public void DeleteHashKey(int storeId, string key, string hashKey)
        {
            GetDb(storeId).HashDelete(key, hashKey);
        }

        private IDatabase GetDb(int storeId)
        {
            try
            {
                if (_redisConnection == null || !_redisConnection.IsConnected)
                    _redisConnection = ConnectionMultiplexer.Connect(_configurationOptions);

                _db = _redisConnection.GetDatabase(storeId);
            }
            catch (System.Exception exception)
            {
                throw exception;
            }

            return _db;
        }
    }
}
