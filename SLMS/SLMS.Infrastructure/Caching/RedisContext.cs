using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SLMS.Infrastructure.Caching
{
    public class RedisContext
    {
        private readonly string _connectionString;
        public RedisContext(Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            _connectionString = configuration["ConnectionStrings:RedisServerUrl"];
        }
        private StackExchange.Redis.ConnectionMultiplexer GetConnection()
        {
            var connection = StackExchange.Redis.ConnectionMultiplexer.Connect(_connectionString);
            return connection;
        }

        /// <summary>
        /// 插入对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        public async Task<bool> SetObjectAsync<T>(string key, T value, TimeSpan? expiry = null)
        {
            await using var connection = GetConnection();
            var db = connection.GetDatabase();
            var serializedValue = JsonConvert.SerializeObject(value);
            await db.StringSetAsync(key, serializedValue, expiry);
            return true;
        }

        /// <summary>
        /// 读取对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<T> GetObjectAsync<T>(string key)
        {
            await using var connection = GetConnection();
            var db = connection.GetDatabase();
            var serializedValue = await db.StringGetAsync(key);
            if (serializedValue.IsNullOrEmpty)
            {
                return default;
            }
            return JsonConvert.DeserializeObject<T>(serializedValue);
        }


        /// <summary>
        /// 向List插入数据
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        public async Task<bool> SetObjectListAsync<T>(string key, T value, TimeSpan? expiry = null)
        {
            await using var connection = GetConnection();
            var db = connection.GetDatabase();
            var serializedValue = JsonConvert.SerializeObject(value);
            await db.ListLeftPushAsync(key, serializedValue);
            if (expiry.HasValue)
            {
                await db.KeyExpireAsync(key, expiry);
            }
            return true;
        }

        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public async Task<bool> ObjectListRemoveAsync(string key, string value)
        {
            await using var connection = GetConnection();
            var db = connection.GetDatabase();
            await db.ListRemoveAsync(key, value);
            return true;
        }

        /// <summary>
        /// 修改缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">键</param>
        /// <param name="oldValue">旧的值</param>
        /// <param name="newValue">新的值</param>
        /// <returns></returns>
        public async Task<bool> ListSetObjectByIndexAsync<T>(string key, T oldValue, T newValue)
        {
            var serializedOldValue = JsonConvert.SerializeObject(oldValue);
            var serializedNewValue = JsonConvert.SerializeObject(newValue);
            await using var connection = GetConnection();
            var db = connection.GetDatabase();
            var length = db.ListLength(key);
            for (var i = 0; i < length; i++)
            {
                var valueIndex = await db.ListGetByIndexAsync(key, i);
                if (!valueIndex.IsNullOrEmpty && valueIndex == serializedOldValue)
                {
                    await db.ListSetByIndexAsync(key, i, serializedNewValue);
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// 读取List中的数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<List<T>> GetObjectListAsync<T>(string key)
        {
            var list = new List<T>();
            await using var connection = GetConnection();

            var db = connection.GetDatabase();
            var len = await db.ListLengthAsync(key);
            for (var i = 0; i < len; i++)
            {
                var serializedValue = await db.ListGetByIndexAsync(key, i);
                if (!serializedValue.IsNullOrEmpty)
                {
                    var value = JsonConvert.DeserializeObject<T>(serializedValue);
                    list.Add(value);
                }
            }
            return list;
        }

        /// <summary>
        /// 设置数据超时的时间
        /// </summary>
        /// <param name="keyName">键</param>
        /// <param name="expiry">超时时间</param>
        public async Task SetTimeoutAsync(string keyName, TimeSpan expiry)
        {
            await using var connection = GetConnection();
            var db = connection.GetDatabase(); 
            await db.KeyExpireAsync(keyName, expiry);
        }
    }
}
