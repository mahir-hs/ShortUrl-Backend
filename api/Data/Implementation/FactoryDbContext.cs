using api.Data.Interface;
using MongoDB.Driver;
using StackExchange.Redis;

namespace api.Data.Implementation
{
    public class FactoryDbContext(IConfiguration configuration) : IFactoryDbContext
    {
        private readonly IConfiguration _configuration = configuration;

        public IMongoDatabase CreateMongoDbConnection()
        {
            try
            {
                var client = new MongoClient(_configuration.GetConnectionString("MongoDbConnection"));
                return client.GetDatabase(_configuration["ConnectionStrings:MongoDbDatabaseName"]);
            }
            catch (MongoException ex)
            {
                throw new Exception("Error while connecting to MongoDB: " + ex.Message, ex);
            }
        }

        public IDatabase CreateRedisConnection()
        {
            try
            {
                var redisConnectionString = _configuration.GetConnectionString("RedisConnection");

                if (string.IsNullOrEmpty(redisConnectionString))
                {
                    throw new ArgumentNullException("RedisConnection", "Redis connection string is missing in configuration.");
                }

                // Optionally, specify retry settings
                var configOptions = ConfigurationOptions.Parse(redisConnectionString);
                configOptions.AbortOnConnectFail = false;  // Will not throw exceptions if Redis is unreachable initially
                configOptions.ConnectTimeout = 10000;      // Set a 10-second connection timeout
                configOptions.SyncTimeout = 10000;         // Set a 10-second synchronous operation timeout

                // Create connection to Redis
                var connection = ConnectionMultiplexer.Connect(configOptions);

                // Return the Redis database instance
                return connection.GetDatabase();
            }
            catch (RedisConnectionException ex)
            {
                Console.WriteLine("Redis connection failed: " + ex.Message);
                throw new Exception("Error while connecting to Redis: " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new Exception("General error while connecting to Redis: " + ex.Message, ex);
            }
        }
    }
}
