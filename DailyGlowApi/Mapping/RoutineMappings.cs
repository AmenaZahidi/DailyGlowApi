using DailyGlowApi.Dtos;
using DailyGlowApi.Models;

namespace DailyGlowApi.Mapping;

// Hand-written mapping for routines and steps
public static class RoutineMappings
{
    public static RoutineSummaryDto ToSummaryDto(this Routine routine)
    {
        return new RoutineSummaryDto(
            routine.Id,
            routine.Name,
            routine.TimeOfDay,
            routine.Steps.Count);
    }

    public static RoutineDetailDto ToDetailDto(this Routine routine)
    {
        var steps = routine.Steps
            .OrderBy(s => s.StepOrder)
            .Select(s => s.ToDto())
            .ToList();

        return new RoutineDetailDto(routine.Id, routine.Name, routine.TimeOfDay, steps);
    }

    // Load the step's Product first: the DTO shows its name and brand
    public static RoutineStepDto ToDto(this RoutineStep step)
    {
        return new RoutineStepDto(
            step.Id,
            step.StepOrder,
            step.ProductId,
            step.Product.Name,
            step.Product.Brand,
            step.Instructions);
    }

    public static Routine ToEntity(this CreateRoutineDto dto)
    {
        var routine = new Routine();
        dto.ApplyTo(routine);
        return routine;
    }

    public static void ApplyTo(this CreateRoutineDto dto, Routine routine)
    {
        routine.Name = dto.Name.Trim();
        routine.TimeOfDay = dto.TimeOfDay ?? TimeOfDay.Morning;
    }

    // Copies the order and instructions. The product is set in the service.
    public static void ApplyTo(this CreateStepDto dto, RoutineStep step)
    {
        step.StepOrder = dto.StepOrder;
        step.Instructions = string.IsNullOrWhiteSpace(dto.Instructions)
            ? null
            : dto.Instructions.Trim();
    }
}
