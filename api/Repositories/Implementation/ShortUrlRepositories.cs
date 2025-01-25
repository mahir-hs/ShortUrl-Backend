using api.Data.Interface;
using api.Models;
using api.Repositories.Interface;
using MongoDB.Driver;

namespace api.Repositories.Implementation
{
    public class ShortUrlRepositories : IShortUrlRepository
    {
        private readonly IMongoCollection<ShortUrl> _shortUrls;

        public ShortUrlRepositories(IFactoryDbContext context)
        {
            var database = context.CreateMongoDbConnection();
            _shortUrls = database.GetCollection<ShortUrl>("Urls");
        }

        public async Task<ApiResponse> CreateAsync(ShortUrl shortUrl)
        {
            var existingShortUrl = await _shortUrls
            .Find(x => x.OriginalUrl == shortUrl.OriginalUrl && x.ExpirationDate == shortUrl.ExpirationDate)
            .FirstOrDefaultAsync();

            if (existingShortUrl != null)
            {
                return new ApiResponse { Success = true, Message = "Short URL already exists" };
            }

            await _shortUrls.InsertOneAsync(shortUrl);
            return new ApiResponse {  Success = true, Message = "Short URL created successfully" };
        }

        public async Task<ApiResponse> DeleteAsync(string id)
        {
            var result = await _shortUrls.DeleteOneAsync(x => x.Id == id);
            if (!result.IsAcknowledged)
            {
                return new ApiResponse { Success = false, Message = "Short URL not found" };
            }
            return new ApiResponse { Success = result.IsAcknowledged, Message = "Short URL deleted successfully" };
        }

        public async Task<ApiResponse> GetByIdAsync(string id)
        {
            var shortUrl = await _shortUrls.Find(x => x.Id == id).FirstOrDefaultAsync();
            if (shortUrl != null)
            {
                return new ApiResponse { Success = true, Message = "Short URL found", Result = shortUrl };
            }
            return new ApiResponse { Success = false, Message = "Short URL not found" };
        }

        public async Task<ApiResponse> GetByShortCodeAsync(string shortCode)
        {
            var shortUrl = await _shortUrls.Find(x => x.ShortCode == shortCode).FirstOrDefaultAsync();
            if (shortUrl != null) {
                return new ApiResponse { Success = true, Message = "Short URL found", Result = shortUrl };
            }
            return new ApiResponse { Success = false, Message = "Short URL not found" };
        }

    }
}
