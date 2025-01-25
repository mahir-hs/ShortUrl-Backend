using MongoDB.Driver;
using StackExchange.Redis;

namespace api.Data.Interface
{
    public interface IFactoryDbContext
    {
        IMongoDatabase CreateMongoDbConnection();
        IDatabase CreateRedisConnection();
    }
}
