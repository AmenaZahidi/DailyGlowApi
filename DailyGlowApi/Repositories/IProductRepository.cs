using DailyGlowApi.Models;

namespace DailyGlowApi.Repositories;

// The only place that talks to the database.
// Using an interface lets us fake it in unit tests.
public interface IProductRepository
{
    Task<IReadOnlyList<Product>> GetAllAsync(
        ProductCategory? category, SkinType? suitableFor, int page, int pageSize);

    Task<Product?> GetByIdAsync(int id);
    Task<Product> AddAsync(Product product);
    Task UpdateAsync(Product product);
    Task DeleteAsync(Product product);
    Task<bool> IsUsedInRoutinesAsync(int productId);
}
