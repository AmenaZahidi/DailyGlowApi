using DailyGlowApi.Models;

namespace DailyGlowApi.Repositories;

// Talks to the database. An interface, so unit tests can fake it.
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
