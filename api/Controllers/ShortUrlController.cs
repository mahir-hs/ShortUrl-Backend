using api.DTOs;
using api.Services.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShortUrlController(IShortUrlService shortUrlService) : ControllerBase
    {
        private readonly IShortUrlService _shortUrlService = shortUrlService;

        [HttpPost("create")]
        public async Task<IActionResult> CreateShortUrl([FromBody] CreateShortUrlRequestDTO request)
        {
            var result = await _shortUrlService.CreateShortUrlAsync(request.OriginalUrl, request.ExpirationDate);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("shortCode")]
        public async Task<IActionResult> GetOriginalUrl([FromQuery]string shortCode)
        {
            var response = await _shortUrlService.GetOriginalUrlAsync(shortCode);

            if (response.Success)
            {
                return Ok(response);
            }

            return NotFound(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteShortUrl(string id)
        {
            var result = await _shortUrlService.DeleteShortUrlAsync(id);
            return result.Success ? Ok(result) : NotFound(new { Message = "Failed to delete URL" });
        }
    }
}
