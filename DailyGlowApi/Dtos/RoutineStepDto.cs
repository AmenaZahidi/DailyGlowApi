namespace DailyGlowApi.Dtos;

// One step, with the product name and brand included
public record RoutineStepDto(
    int Id,
    int StepOrder,
    int ProductId,
    string ProductName,
    string Brand,
    string? Instructions);
