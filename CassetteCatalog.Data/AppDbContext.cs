using CassetteCatalog.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace CassetteCatalog.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Album> Albums => Set<Album>();
        public DbSet<Track> Tracks => Set<Track>();

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Album>(e =>
            {
                e.HasMany(a => a.Tracks)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);

                e.Property(p => p.TapeType)
                .HasConversion<int>();
            });

            modelBuilder.Entity<Track>(e =>
            {
                e.Property(p => p.Side)
                .HasConversion<int>();
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
