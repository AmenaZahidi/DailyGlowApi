using DailyGlowApi.Models;

namespace DailyGlowApi.Dtos;

// What the API sends back. We never send the database class directly.
public record ProductDto(
    int Id,
    string Name,
    string Brand,
    ProductCategory Category,
    ActiveIngredient ActiveIngredient,
    SkinType SuitableFor,
    decimal? Price);
