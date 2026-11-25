using DailyGlowApi.Dtos;

namespace DailyGlowApi.Services;

public enum DeleteProductResult
{
    Deleted,
    NotFound,
    InUse
}

public interface IProductService
{
    Task<IReadOnlyList<ProductDto>> GetAllAsync(ProductQueryParameters query);
    Task<ProductDto?> GetByIdAsync(int id);
    Task<ProductDto> CreateAsync(CreateProductDto dto);
    Task<bool> UpdateAsync(int id, UpdateProductDto dto);
    Task<DeleteProductResult> DeleteAsync(int id);
}
