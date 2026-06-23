using urlshortner.Models;
using urlshortner.Interfaces;
using urlshortner.Database;
using Microsoft.Extensions.Caching.Memory;
//using Microsoft.Extensions.Caching.Distributed;

namespace urlshortner.Services
{
    public class UrlService : IUrlService
    {
        private readonly IMemoryCache _cache;

        //private readonly IDistributedCache _distributedCache;

        private readonly AppDbContext _context;
        
        public UrlService(AppDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<UrlMappings> CreateShortUrl(string originalUrl)
        {
            var entity = new UrlMappings
            {
                OriginalUrl = originalUrl,
                ShortenedUrl = string.Empty,
                CreatedAt = DateTime.UtcNow
            };

            _context.UrlMappings.Add(entity);
            await _context.SaveChangesAsync();

            string shortenedUrl = GenerateShortenedUrl(entity.Id);

            entity.ShortenedUrl = shortenedUrl;
            await _context.SaveChangesAsync();

            return entity;
        }

        public async Task<string> GetOriginalUrlAsync(string shortenedUrl)
        {
            if (_cache.TryGetValue(shortenedUrl, out string? cachedUrl) && !string.IsNullOrEmpty(cachedUrl))
            {
                return cachedUrl;
            }
            var originalUrl = _context.UrlMappings.FirstOrDefault(u => u.ShortenedUrl == shortenedUrl)?.OriginalUrl ?? string.Empty;
            if (!string.IsNullOrEmpty(originalUrl))
            {
                _cache.Set(shortenedUrl, originalUrl, TimeSpan.FromHours(1));
            }
            return originalUrl;
        }

        //public async Task<string> GetRedisOriginalUrlAsync(string shortCode)
        //{
        //    var cached = await _distributedCache.GetStringAsync(shortCode);

        //    if (!string.IsNullOrEmpty(cached))
        //        return cached;

        //    var url = _context.UrlMappings
        //        .FirstOrDefault(x => x.ShortenedUrl == shortCode)?.OriginalUrl ?? string.Empty;

        //    if (!string.IsNullOrEmpty(url))
        //    {
        //        await _distributedCache.SetStringAsync(shortCode, url,
        //            new DistributedCacheEntryOptions
        //            {
        //                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
        //            });
        //    }

        //    return url;
        //}

        private static string GenerateShortenedUrl(int id)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var result = "";
            while (id > 0)
            {
                result = chars[id % chars.Length] + result;
                id /= chars.Length;
            }
            return result; 
        }
    }
}