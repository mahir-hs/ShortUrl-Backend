using api.Data.Interface;
using api.Repositories.Interface;
using StackExchange.Redis;

namespace api.Repositories.Implementation
{
    public class RedisRepository(IFactoryDbContext factoryDbContext) : IRedisRepository
    {
        private readonly IDatabase _redisDb = factoryDbContext.CreateRedisConnection();

        public async Task SetAsync(string key, string value, TimeSpan? expiration = null)
        {
            try
            {
                if (expiration.HasValue)
                {
                    await _redisDb.StringSetAsync(key, value, expiration);
                }
                else
                {
                    await _redisDb.StringSetAsync(key, value);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while setting the key '{key}' in Redis.", ex);
            }
        }

        public async Task<string?> GetAsync(string key)
        {
            try
            {
                var value = await _redisDb.StringGetAsync(key);
                return value.HasValue ? value.ToString() : null;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while getting the key '{key}' from Redis.", ex);
            }
        }

        public async Task<bool> DeleteAsync(string key)
        {
            try
            {
                return await _redisDb.KeyDeleteAsync(key);
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while deleting the key '{key}' from Redis.", ex);
            }
        }

        public async Task<bool> KeyExistsAsync(string key)
        {
            try
            {
                return await _redisDb.KeyExistsAsync(key);
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while checking if the key '{key}' exists in Redis.", ex);
            }
        }
    }
}
