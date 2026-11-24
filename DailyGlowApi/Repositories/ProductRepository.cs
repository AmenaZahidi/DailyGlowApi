using DailyGlowApi.Data;
using DailyGlowApi.Models;
using Microsoft.EntityFrameworkCore;

namespace DailyGlowApi.Repositories;

// Database code for products, using EF Core
public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _db;

    public ProductRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<Product>> GetAllAsync(
        ProductCategory? category, SkinType? suitableFor, int page, int pageSize)
    {
        var query = _db.Products.AsNoTracking();

        if (category.HasValue)
        {
            query = query.Where(p => p.Category == category.Value);
        }

        if (suitableFor.HasValue)
        {
            // "All" products fit every skin type
            query = query.Where(p =>
                p.SuitableFor == suitableFor.Value || p.SuitableFor == SkinType.All);
        }

        return await query
            .OrderBy(p => p.Name)
            .ThenBy(p => p.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        return await _db.Products.FindAsync(id);
    }

    public async Task<Product> AddAsync(Product product)
    {
        _db.Products.Add(product);
        await _db.SaveChangesAsync();
        return product;
    }

    public async Task UpdateAsync(Product product)
    {
        _db.Products.Update(product);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(Product product)
    {
        _db.Products.Remove(product);
        await _db.SaveChangesAsync();
    }
}
