using System.ComponentModel.DataAnnotations;

namespace DailyGlowApi.Dtos;

// What the client sends when adding a step to a routine
public class CreateStepDto
{
    [Range(1, int.MaxValue)]
    public int ProductId { get; set; }

    [Range(1, int.MaxValue)]
    public int StepOrder { get; set; }

    [StringLength(250)]
    public string? Instructions { get; set; }
}
