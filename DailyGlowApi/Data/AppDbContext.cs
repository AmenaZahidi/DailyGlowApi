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
    public DbSet<Routine> Routines => Set<Routine>();
    public DbSet<RoutineStep> RoutineSteps => Set<RoutineStep>();
    public DbSet<User> Users => Set<User>();

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

        modelBuilder.Entity<Routine>(entity =>
        {
            entity.Property(r => r.Name).IsRequired().HasMaxLength(100);
            entity.Property(r => r.TimeOfDay).HasConversion<string>().HasMaxLength(30);
        });

        modelBuilder.Entity<RoutineStep>(entity =>
        {
            entity.Property(s => s.Instructions).HasMaxLength(250);

            // One routine has many steps. Deleting a routine deletes its steps.
            entity.HasOne(s => s.Routine)
                .WithMany(r => r.Steps)
                .HasForeignKey(s => s.RoutineId)
                .OnDelete(DeleteBehavior.Cascade);

            // One product can be used in many steps. A used product cannot be deleted.
            entity.HasOne(s => s.Product)
                .WithMany()
                .HasForeignKey(s => s.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // Two steps in the same routine cannot have the same order number
            entity.HasIndex(s => new { s.RoutineId, s.StepOrder }).IsUnique();
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(u => u.Email).IsRequired().HasMaxLength(254);
            entity.Property(u => u.PasswordHash).IsRequired().HasMaxLength(255);

            // Each email can be registered only once
            entity.HasIndex(u => u.Email).IsUnique();
        });
    }
}
