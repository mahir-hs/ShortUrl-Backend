using api.Models;

namespace api.Repositories.Interface
{
    public interface IShortUrlRepository
    {
        Task<ApiResponse> GetByShortCodeAsync(string shortCode);
        Task<ApiResponse> CreateAsync(ShortUrl shortUrl);
        Task<ApiResponse> DeleteAsync(string id);
        Task<ApiResponse> GetByIdAsync(string id);
    }
}
