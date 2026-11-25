namespace DailyGlowApi.Models;

public enum ProductCategory
{
    Cleanser,
    Toner,
    Serum,
    Moisturiser,
    Sunscreen,
    Treatment,
    Mask,
    Other
}

public enum ActiveIngredient
{
    None,
    Retinol,
    Aha,
    Bha,
    VitaminC,
    BenzoylPeroxide,
    Niacinamide,
    HyaluronicAcid
}

public enum SkinType
{
    All,
    Dry,
    Oily,
    Combination,
    Sensitive
}

public enum TimeOfDay
{
    Morning,
    Evening
}
