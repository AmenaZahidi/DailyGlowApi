using DailyGlowApi.Dtos;
using DailyGlowApi.Models;

namespace DailyGlowApi.Mapping;

// Hand-written mapping between the database class and the DTOs
public static class ProductMappings
{
    public static ProductDto ToDto(this Product product)
    {
        return new ProductDto(
            product.Id,
            product.Name,
            product.Brand,
            product.Category,
            product.ActiveIngredient,
            product.SuitableFor,
            product.Price);
    }

    public static Product ToEntity(this CreateProductDto dto)
    {
        var product = new Product();
        dto.ApplyTo(product);
        return product;
    }

    // Copies the DTO values onto a product (used for create and update)
    public static void ApplyTo(this CreateProductDto dto, Product product)
    {
        product.Name = dto.Name.Trim();
        product.Brand = dto.Brand.Trim();
        product.Category = dto.Category ?? ProductCategory.Other;
        product.ActiveIngredient = dto.ActiveIngredient;
        product.SuitableFor = dto.SuitableFor;
        product.Price = dto.Price;
    }
}
