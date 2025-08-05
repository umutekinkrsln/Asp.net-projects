using Microsoft.EntityFrameworkCore;
using MapApp.Models;

namespace MapApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<MapPoints> MapPoints => Set<MapPoints>();
    }
}

