namespace DailyGlowApi.Models;

// One skincare product. EF Core turns this class into the Products table.
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public ProductCategory Category { get; set; }
    public ActiveIngredient ActiveIngredient { get; set; } = ActiveIngredient.None;
    public SkinType SuitableFor { get; set; } = SkinType.All;

    // Optional
    public decimal? Price { get; set; }
}
