using DailyGlowApi.Dtos;
using DailyGlowApi.Mapping;
using DailyGlowApi.Repositories;

namespace DailyGlowApi.Services;

// The logic of the app. The controller calls this, and this calls the repository.
public class ProductService : IProductService
{
    private readonly IProductRepository _repository;

    public ProductService(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<ProductDto>> GetAllAsync(ProductQueryParameters query)
    {
        var products = await _repository.GetAllAsync(
            query.Category, query.SkinType, query.Page, query.PageSize);

        return products.Select(p => p.ToDto()).ToList();
    }

    public async Task<ProductDto?> GetByIdAsync(int id)
    {
        var product = await _repository.GetByIdAsync(id);
        return product?.ToDto();
    }

    public async Task<ProductDto> CreateAsync(CreateProductDto dto)
    {
        var product = dto.ToEntity();
        var saved = await _repository.AddAsync(product);
        return saved.ToDto();
    }

    public async Task<bool> UpdateAsync(int id, UpdateProductDto dto)
    {
        var product = await _repository.GetByIdAsync(id);
        if (product is null)
        {
            return false;
        }

        dto.ApplyTo(product);
        await _repository.UpdateAsync(product);
        return true;
    }

    public async Task<DeleteProductResult> DeleteAsync(int id)
    {
        var product = await _repository.GetByIdAsync(id);
        if (product is null)
        {
            return DeleteProductResult.NotFound;
        }

        await _repository.DeleteAsync(product);
        return DeleteProductResult.Deleted;
    }
}
