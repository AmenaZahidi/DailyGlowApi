using DailyGlowApi.Dtos;
using DailyGlowApi.Models;
using DailyGlowApi.Repositories;
using DailyGlowApi.Services;
using Moq;
using System.Timers;
using Xunit;

namespace DailyGlowApi.Tests;

// Unit tests for ProductService. The repository is faked with Moq, so there is no database.
// Based on Microsoft Learn: Unit testing C# with xUnit
// https://learn.microsoft.com/dotnet/core/testing/unit-testing-csharp-with-xunit
// and the Moq Quickstart: https://github.com/devlooped/moq/wiki/Quickstart
public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _repo = new();
    private readonly ProductService _service;

    public ProductServiceTests()
    {
        _service = new ProductService(_repo.Object);
    }

    private static Product MakeProduct(int id = 1, string name = "Gentle Cleanser")
    {
        return new Product
        {
            Id = id,
            Name = name,
            Brand = "Cera",
            Category = ProductCategory.Cleanser,
            ActiveIngredient = ActiveIngredient.None,
            SuitableFor = SkinType.All,
            Price = 9.99m
        };
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenProductDoesNotExist()
    {
        // Arrange
        _repo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Product?)null);

        // Act
        var result = await _service.GetByIdAsync(99);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsDto_WhenProductExists()
    {
        _repo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(MakeProduct());

        var result = await _service.GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal(1, result!.Id);
        Assert.Equal("Gentle Cleanser", result.Name);
        Assert.Equal("Cera", result.Brand);
        Assert.Equal(ProductCategory.Cleanser, result.Category);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsOneDtoPerProduct()
    {
        IReadOnlyList<Product> products = new List<Product>
        {
            MakeProduct(1, "Gentle Cleanser"),
            MakeProduct(2, "Vitamin C Serum")
        };
        _repo.Setup(r => r.GetAllAsync(
                It.IsAny<ProductCategory?>(),
                It.IsAny<SkinType?>(),
                It.IsAny<int>(),
                It.IsAny<int>()))
            .ReturnsAsync(products);

        var result = await _service.GetAllAsync(new ProductQueryParameters());

        Assert.Equal(2, result.Count);
        Assert.Equal("Gentle Cleanser", result[0].Name);
        Assert.Equal("Vitamin C Serum", result[1].Name);
    }

    [Fact]
    public async Task GetAllAsync_PassesFiltersAndPagingToTheRepository()
    {
        IReadOnlyList<Product> none = new List<Product>();
        _repo.Setup(r => r.GetAllAsync(
                It.IsAny<ProductCategory?>(),
                It.IsAny<SkinType?>(),
                It.IsAny<int>(),
                It.IsAny<int>()))
            .ReturnsAsync(none);
        var query = new ProductQueryParameters
        {
            Category = ProductCategory.Serum,
            SkinType = SkinType.Oily,
            Page = 2,
            PageSize = 5
        };

        await _service.GetAllAsync(query);

        _repo.Verify(r => r.GetAllAsync(ProductCategory.Serum, SkinType.Oily, 2, 5), Times.Once());
    }

    [Fact]
    public async Task CreateAsync_SavesTrimmedProduct_AndReturnsDtoWithNewId()
    {
        Product? saved = null;

        // Pretend to be the database: give the new product an Id
        _repo.Setup(r => r.AddAsync(It.IsAny<Product>()))
            .ReturnsAsync((Product product) =>
            {
                saved = product;
                product.Id = 7;
                return product;
            });

        var dto = new CreateProductDto
        {
            Name = "  Vitamin C Serum  ",
            Brand = " Ordinary ",
            Category = ProductCategory.Serum,
            ActiveIngredient = ActiveIngredient.VitaminC,
            SuitableFor = SkinType.Oily,
            Price = 12.5m
        };

        var result = await _service.CreateAsync(dto);

        Assert.Equal(7, result.Id);
        Assert.Equal("Vitamin C Serum", result.Name);
        Assert.Equal("Ordinary", result.Brand);
        Assert.NotNull(saved);
        Assert.Equal(ActiveIngredient.VitaminC, saved!.ActiveIngredient);
        _repo.Verify(r => r.AddAsync(It.IsAny<Product>()), Times.Once());
    }

    [Fact]
    public async Task UpdateAsync_ReturnsFalse_WhenProductDoesNotExist()
    {
        _repo.Setup(r => r.GetByIdAsync(5)).ReturnsAsync((Product?)null);
        var dto = new UpdateProductDto { Name = "Anything", Brand = "Any", Category = ProductCategory.Toner };

        var ok = await _service.UpdateAsync(5, dto);

        Assert.False(ok);
        _repo.Verify(r => r.UpdateAsync(It.IsAny<Product>()), Times.Never());
    }

    [Fact]
    public async Task UpdateAsync_ChangesTheProduct_WhenItExists()
    {
        var existing = MakeProduct(3, "Old Name");
        _repo.Setup(r => r.GetByIdAsync(3)).ReturnsAsync(existing);
        var dto = new UpdateProductDto
        {
            Name = " New Name ",
            Brand = "New Brand",
            Category = ProductCategory.Moisturiser,
            ActiveIngredient = ActiveIngredient.HyaluronicAcid,
            SuitableFor = SkinType.Dry,
            Price = 20m
        };

        var ok = await _service.UpdateAsync(3, dto);

        Assert.True(ok);
        Assert.Equal("New Name", existing.Name);
        Assert.Equal(ProductCategory.Moisturiser, existing.Category);
        Assert.Equal(SkinType.Dry, existing.SuitableFor);
        _repo.Verify(r => r.UpdateAsync(existing), Times.Once());
    }

    [Fact]
    public async Task DeleteAsync_ReturnsNotFound_WhenProductDoesNotExist()
    {
        _repo.Setup(r => r.GetByIdAsync(8)).ReturnsAsync((Product?)null);

        var result = await _service.DeleteAsync(8);

        Assert.Equal(DeleteProductResult.NotFound, result);
        _repo.Verify(r => r.DeleteAsync(It.IsAny<Product>()), Times.Never());
    }

    [Fact]
    public async Task DeleteAsync_ReturnsInUse_WhenProductIsUsedInARoutine()
    {
        var existing = MakeProduct(4);
        _repo.Setup(r => r.GetByIdAsync(4)).ReturnsAsync(existing);
        _repo.Setup(r => r.IsUsedInRoutinesAsync(4)).ReturnsAsync(true);

        var result = await _service.DeleteAsync(4);

        Assert.Equal(DeleteProductResult.InUse, result);
        _repo.Verify(r => r.DeleteAsync(It.IsAny<Product>()), Times.Never());
    }

    [Fact]
    public async Task DeleteAsync_DeletesTheProduct_WhenItIsNotUsed()
    {
        var existing = MakeProduct(4);
        _repo.Setup(r => r.GetByIdAsync(4)).ReturnsAsync(existing);
        _repo.Setup(r => r.IsUsedInRoutinesAsync(4)).ReturnsAsync(false);

        var result = await _service.DeleteAsync(4);

        Assert.Equal(DeleteProductResult.Deleted, result);
        _repo.Verify(r => r.DeleteAsync(existing), Times.Once());
    }
}