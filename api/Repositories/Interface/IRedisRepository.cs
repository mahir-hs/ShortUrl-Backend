namespace api.Repositories.Interface
{
    public interface IRedisRepository
    {
        Task SetAsync(string key, string value, TimeSpan? expiration = null);
        Task<string?> GetAsync(string key);
        Task<bool> DeleteAsync(string key);
        Task<bool> KeyExistsAsync(string key);
    }
}
