using DailyGlowApi.Dtos;

namespace DailyGlowApi.Services;

// The possible results when adding or changing a step
public enum StepStatus
{
    Success,
    RoutineNotFound,
    StepNotFound,
    ProductNotFound,
    OrderTaken
}

public record StepResult(StepStatus Status, RoutineStepDto? Step = null);

public interface IRoutineService
{
    Task<IReadOnlyList<RoutineSummaryDto>> GetAllAsync();
    Task<RoutineDetailDto?> GetByIdAsync(int id);
    Task<RoutineDetailDto> CreateAsync(CreateRoutineDto dto);
    Task<bool> UpdateAsync(int id, UpdateRoutineDto dto);
    Task<bool> DeleteAsync(int id);

    // Returns null when the routine does not exist
    Task<IReadOnlyList<RoutineStepDto>?> GetStepsAsync(int routineId);
    Task<StepResult> AddStepAsync(int routineId, CreateStepDto dto);
    Task<StepResult> UpdateStepAsync(int routineId, int stepId, UpdateStepDto dto);
    Task<bool> DeleteStepAsync(int routineId, int stepId);
}
