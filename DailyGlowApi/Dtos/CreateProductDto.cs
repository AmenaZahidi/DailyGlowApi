using System.ComponentModel.DataAnnotations;
using DailyGlowApi.Models;

namespace DailyGlowApi.Dtos;

// What the client sends when creating a product
public class CreateProductDto
{
    [Required, StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required, StringLength(100)]
    public string Brand { get; set; } = string.Empty;

    // Nullable, so a missing category gives an error
    [Required]
    public ProductCategory? Category { get; set; }

    public ActiveIngredient ActiveIngredient { get; set; } = ActiveIngredient.None;

    public SkinType SuitableFor { get; set; } = SkinType.All;

    [Range(typeof(decimal), "0", "100000")]
    public decimal? Price { get; set; }
}
