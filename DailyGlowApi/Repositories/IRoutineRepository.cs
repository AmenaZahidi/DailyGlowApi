using DailyGlowApi.Models;

namespace DailyGlowApi.Repositories;

// Database operations for routines and their steps (one repository for both).
public interface IRoutineRepository
{
    Task<IReadOnlyList<Routine>> GetAllAsync();
    Task<Routine?> GetByIdAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task<Routine> AddAsync(Routine routine);
    Task UpdateAsync(Routine routine);
    Task DeleteAsync(Routine routine);

    Task<IReadOnlyList<RoutineStep>> GetStepsAsync(int routineId);
    Task<RoutineStep?> GetStepAsync(int routineId, int stepId);
    Task<bool> StepOrderTakenAsync(int routineId, int stepOrder, int? exceptStepId);
    Task<RoutineStep> AddStepAsync(RoutineStep step);
    Task UpdateStepAsync(RoutineStep step);
    Task DeleteStepAsync(RoutineStep step);
}
