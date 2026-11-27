using System.ComponentModel.DataAnnotations;
using DailyGlowApi.Models;

namespace DailyGlowApi.Dtos;

// Optional filters and paging, e.g. ?category=Serum&page=1
public class ProductQueryParameters
{
    public ProductCategory? Category { get; set; }
    public SkinType? SkinType { get; set; }

    [Range(1, int.MaxValue)]
    public int Page { get; set; } = 1;

    [Range(1, 100)]
    public int PageSize { get; set; } = 20;
}
