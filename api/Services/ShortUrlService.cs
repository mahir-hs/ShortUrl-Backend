using api.Models;
using api.Repositories.Interface;
using api.Services.IService;
using System.Text;

namespace api.Services
{
    public class ShortUrlService(IShortUrlRepository shortUrlRepository, IRedisRepository redisRepository, ILogger<ShortUrlService> logger, IConfiguration configuration) : IShortUrlService
    {
        private readonly IShortUrlRepository _shortUrlRepository = shortUrlRepository;
        private readonly IRedisRepository _redisRepository = redisRepository;
        private readonly ILogger<ShortUrlService> _logger = logger;
        private readonly string _testDomain = configuration["AppSettings:TestDomain"]!;

        public async Task<ApiResponse> CreateShortUrlAsync(string originalUrl, DateTime? expirationDate = null)
        {
            if (string.IsNullOrEmpty(originalUrl))
            {
                throw new ArgumentException("Original URL cannot be null or empty.", nameof(originalUrl));
            }

            var shortCode = GenerateShortCode();
            if (string.IsNullOrEmpty(shortCode))
            {
                throw new InvalidOperationException("Failed to generate a short code.");
            }

            var shortUrl = new ShortUrl
            {
                OriginalUrl = originalUrl,
                ShortCode = shortCode,
                ExpirationDate = expirationDate ?? DateTime.UtcNow.AddMonths(6),
                CreatedAt = DateTime.UtcNow
            };

            await _shortUrlRepository.CreateAsync(shortUrl);

            await _redisRepository.SetAsync( shortCode, originalUrl, TimeSpan.FromDays(183));

            _logger.LogInformation("Created new short URL: {ShortCode}", shortCode);
            return new ApiResponse { Result = $"{_testDomain}/{shortCode}", Success = true };
        }

        public async Task<ApiResponse> DeleteShortUrlAsync(string id)
        {
            var shortUrl = await _shortUrlRepository.GetByShortCodeAsync(id);
            

            if (shortUrl.Success)
            {
                var success = await _shortUrlRepository.DeleteAsync(shortUrl.Result!.Id);
                await _redisRepository.DeleteAsync(shortUrl.Result!.ShortCode);
                return success;
            }

            return shortUrl;
        }

        public async Task<ApiResponse?> GetByIdAsync(string id)
        {
            return await _shortUrlRepository.GetByIdAsync(id);
        }

        public async Task<ApiResponse?> GetOriginalUrlAsync(string shortCode)
        {
            if (string.IsNullOrEmpty(shortCode))
            {
                return new ApiResponse { Success = false, Message = "Short code cannot be null or empty." };
            }

            var cachedOriginalUrl = await _redisRepository.GetAsync(shortCode);
            if (cachedOriginalUrl != null)
            {
                
                return new ApiResponse { Result = cachedOriginalUrl, Success = true };
            }

            var existingShortUrl = await _shortUrlRepository.GetByShortCodeAsync(shortCode);
            if (existingShortUrl != null && existingShortUrl.Result != null)
            {
                TimeSpan? expiry = null;
                if (existingShortUrl?.Result?.ExpirationDate.HasValue)
                {
                    expiry = existingShortUrl?.Result?.ExpirationDate.Value - DateTime.UtcNow;
                }
                await _redisRepository.SetAsync(shortCode, existingShortUrl?.Result?.OriginalUrl, expiry);
                return existingShortUrl;
            }

            _logger.LogWarning("No URL mapping found for short code: {ShortCode}", shortCode);
            return new ApiResponse { Success = false, Message = "No URL mapping found for the provided short code." };
        }


        private static string GenerateShortCode()
        {
            var id = Math.Abs(Guid.NewGuid().GetHashCode());

            const string chars = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var shortCode = new StringBuilder();

            if (id == 0)
            {
                shortCode.Append(chars[0]);
            }

            while (id > 0)
            {
                shortCode.Insert(0, chars[id % 62]);
                id /= 62;
            }
            var paddedShortCode = shortCode.ToString().PadLeft(6, '0');
            return paddedShortCode;
        }
    }
}
