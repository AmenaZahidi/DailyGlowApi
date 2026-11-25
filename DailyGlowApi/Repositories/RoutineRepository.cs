using DailyGlowApi.Data;
using DailyGlowApi.Models;
using Microsoft.EntityFrameworkCore;

namespace DailyGlowApi.Repositories;

// Database code for routines and steps, using EF Core
public class RoutineRepository : IRoutineRepository
{
    private readonly AppDbContext _db;

    public RoutineRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<Routine>> GetAllAsync()
    {
        // Include the steps so we can count them
        return await _db.Routines
            .AsNoTracking()
            .Include(r => r.Steps)
            .OrderBy(r => r.Name)
            .ThenBy(r => r.Id)
            .ToListAsync();
    }

    public async Task<Routine?> GetByIdAsync(int id)
    {
        // Include the steps and each step's product
        return await _db.Routines
            .Include(r => r.Steps)
            .ThenInclude(s => s.Product)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _db.Routines.AnyAsync(r => r.Id == id);
    }

    public async Task<Routine> AddAsync(Routine routine)
    {
        _db.Routines.Add(routine);
        await _db.SaveChangesAsync();
        return routine;
    }

    public async Task UpdateAsync(Routine routine)
    {
        _db.Routines.Update(routine);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(Routine routine)
    {
        // The steps are deleted too (cascade delete)
        _db.Routines.Remove(routine);
        await _db.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<RoutineStep>> GetStepsAsync(int routineId)
    {
        return await _db.RoutineSteps
            .AsNoTracking()
            .Include(s => s.Product)
            .Where(s => s.RoutineId == routineId)
            .OrderBy(s => s.StepOrder)
            .ToListAsync();
    }

    public async Task<RoutineStep?> GetStepAsync(int routineId, int stepId)
    {
        return await _db.RoutineSteps
            .Include(s => s.Product)
            .FirstOrDefaultAsync(s => s.RoutineId == routineId && s.Id == stepId);
    }

    public async Task<bool> StepOrderTakenAsync(int routineId, int stepOrder, int? exceptStepId)
    {
        var query = _db.RoutineSteps
            .Where(s => s.RoutineId == routineId && s.StepOrder == stepOrder);

        // When updating a step, ignore the step itself
        if (exceptStepId.HasValue)
        {
            query = query.Where(s => s.Id != exceptStepId.Value);
        }

        return await query.AnyAsync();
    }

    public async Task<RoutineStep> AddStepAsync(RoutineStep step)
    {
        _db.RoutineSteps.Add(step);
        await _db.SaveChangesAsync();
        return step;
    }

    public async Task UpdateStepAsync(RoutineStep step)
    {
        _db.RoutineSteps.Update(step);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteStepAsync(RoutineStep step)
    {
        _db.RoutineSteps.Remove(step);
        await _db.SaveChangesAsync();
    }
}
