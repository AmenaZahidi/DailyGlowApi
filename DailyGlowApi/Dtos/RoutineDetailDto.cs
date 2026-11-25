using DailyGlowApi.Models;

namespace DailyGlowApi.Dtos;

// One routine with its steps in order
public record RoutineDetailDto(
    int Id,
    string Name,
    TimeOfDay TimeOfDay,
    IReadOnlyList<RoutineStepDto> Steps);
