namespace urlshortner.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using urlshortner.DTO;
    using urlshortner.Interfaces;

    [ApiController]
    [Route("api/[controller]")]
    public class UrlController : ControllerBase
    {
        private readonly ILogger<UrlController> _logger;
        private readonly IUrlService _urlService;

        public UrlController(IUrlService urlService, ILogger<UrlController> logger)
        {
            _urlService = urlService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> CreateShortUrl([FromBody] CreateUrlRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.OriginalUrl))
            {
                _logger.LogWarning("Original URL is required.");
                return BadRequest("Original URL is required.");
            }

            _logger.LogInformation("Creating short URL for {Url}", request.OriginalUrl);
            var result = await _urlService.CreateShortUrl(request.OriginalUrl);
            var shortUrl = $"{Request.Scheme}://{Request.Host}/{result.ShortenedUrl}";
            return Ok(new CreateUrlResponse
            {
                ShortenedUrl = shortUrl
            });
        }

        [HttpGet("/{code}")]
        public async Task<IActionResult> RedirectToOriginal(string code)
        {
            var originalUrl = await _urlService.GetOriginalUrlAsync(code);
            if (string.IsNullOrEmpty(originalUrl))
            {
                return NotFound("Shortened URL not found.");
            }
            return Redirect(originalUrl);
        }
    }
}