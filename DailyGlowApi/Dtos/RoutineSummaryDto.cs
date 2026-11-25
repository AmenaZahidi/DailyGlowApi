using DailyGlowApi.Models;

namespace DailyGlowApi.Dtos;

// One line in the list of routines
public record RoutineSummaryDto(int Id, string Name, TimeOfDay TimeOfDay, int StepCount);
