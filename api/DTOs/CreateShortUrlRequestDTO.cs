namespace api.DTOs
{
    public class CreateShortUrlRequestDTO
    {
        public required string OriginalUrl { get; set; }
        public DateTime? ExpirationDate { get; set; }
    }
}
