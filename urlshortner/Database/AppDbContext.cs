using Microsoft.EntityFrameworkCore;
using urlshortner.Models;

namespace urlshortner.Database
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<UrlMappings> UrlMappings { get; set; }
    }
}