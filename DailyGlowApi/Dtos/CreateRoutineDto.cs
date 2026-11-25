using System.ComponentModel.DataAnnotations;
using DailyGlowApi.Models;

namespace DailyGlowApi.Dtos;

// What the client sends when creating a routine
public class CreateRoutineDto
{
    [Required, StringLength(100)]
    public string Name { get; set; } = string.Empty;

    // Nullable, so a missing value gives an error
    [Required]
    public TimeOfDay? TimeOfDay { get; set; }
}
