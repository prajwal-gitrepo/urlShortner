using urlshortner.Models;

namespace urlshortner.Interfaces
{
    public interface IUrlService
    {
        Task<UrlMappings> CreateShortUrl(string originalUrl);
        Task<string> GetOriginalUrlAsync(string shortenedUrl);
    }
}