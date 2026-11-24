using DailyGlowApi.Models;
using Microsoft.EntityFrameworkCore;

namespace DailyGlowApi.Data;

// The link between our C# classes and the SQLite database
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(entity =>
        {
            entity.Property(p => p.Name).IsRequired().HasMaxLength(100);
            entity.Property(p => p.Brand).IsRequired().HasMaxLength(100);

            // Save enums as words, not numbers
            entity.Property(p => p.Category).HasConversion<string>().HasMaxLength(30);
            entity.Property(p => p.ActiveIngredient).HasConversion<string>().HasMaxLength(30);
            entity.Property(p => p.SuitableFor).HasConversion<string>().HasMaxLength(30);

            entity.Property(p => p.Price).HasPrecision(8, 2);
        });
    }
}
