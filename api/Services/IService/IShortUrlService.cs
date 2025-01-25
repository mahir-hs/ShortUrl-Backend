using api.Models;

namespace api.Services.IService
{
    public interface IShortUrlService
    {
        Task<ApiResponse> CreateShortUrlAsync(string originalUrl, DateTime? expirationDate = null);
        Task<ApiResponse?> GetOriginalUrlAsync(string shortCode);
        Task<ApiResponse?> GetByIdAsync(string id);
        Task<ApiResponse> DeleteShortUrlAsync(string id);
    }
}
