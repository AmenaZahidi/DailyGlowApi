using DailyGlowApi.Dtos;
using DailyGlowApi.Mapping;
using DailyGlowApi.Models;
using DailyGlowApi.Repositories;

namespace DailyGlowApi.Services;

// The logic for routines and steps, including the step rules
public class RoutineService : IRoutineService
{
    private readonly IRoutineRepository _routines;
    private readonly IProductRepository _products;

    public RoutineService(IRoutineRepository routines, IProductRepository products)
    {
        _routines = routines;
        _products = products;
    }

    public async Task<IReadOnlyList<RoutineSummaryDto>> GetAllAsync()
    {
        var routines = await _routines.GetAllAsync();
        return routines.Select(r => r.ToSummaryDto()).ToList();
    }

    public async Task<RoutineDetailDto?> GetByIdAsync(int id)
    {
        var routine = await _routines.GetByIdAsync(id);
        return routine?.ToDetailDto();
    }

    public async Task<RoutineDetailDto> CreateAsync(CreateRoutineDto dto)
    {
        var routine = dto.ToEntity();
        var saved = await _routines.AddAsync(routine);
        return saved.ToDetailDto();
    }

    public async Task<bool> UpdateAsync(int id, UpdateRoutineDto dto)
    {
        var routine = await _routines.GetByIdAsync(id);
        if (routine is null)
        {
            return false;
        }

        dto.ApplyTo(routine);
        await _routines.UpdateAsync(routine);
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var routine = await _routines.GetByIdAsync(id);
        if (routine is null)
        {
            return false;
        }

        await _routines.DeleteAsync(routine);
        return true;
    }

    public async Task<IReadOnlyList<RoutineStepDto>?> GetStepsAsync(int routineId)
    {
        if (!await _routines.ExistsAsync(routineId))
        {
            return null;
        }

        var steps = await _routines.GetStepsAsync(routineId);
        return steps.Select(s => s.ToDto()).ToList();
    }

    public async Task<StepResult> AddStepAsync(int routineId, CreateStepDto dto)
    {
        if (!await _routines.ExistsAsync(routineId))
        {
            return new StepResult(StepStatus.RoutineNotFound);
        }

        var product = await _products.GetByIdAsync(dto.ProductId);
        if (product is null)
        {
            return new StepResult(StepStatus.ProductNotFound);
        }

        if (await _routines.StepOrderTakenAsync(routineId, dto.StepOrder, null))
        {
            return new StepResult(StepStatus.OrderTaken);
        }

        var step = new RoutineStep
        {
            RoutineId = routineId,
            ProductId = product.Id,
            Product = product
        };
        dto.ApplyTo(step);

        var saved = await _routines.AddStepAsync(step);
        return new StepResult(StepStatus.Success, saved.ToDto());
    }

    public async Task<StepResult> UpdateStepAsync(int routineId, int stepId, UpdateStepDto dto)
    {
        var step = await _routines.GetStepAsync(routineId, stepId);
        if (step is null)
        {
            return new StepResult(StepStatus.StepNotFound);
        }

        // Only look up the product when it changed
        if (dto.ProductId != step.ProductId)
        {
            var product = await _products.GetByIdAsync(dto.ProductId);
            if (product is null)
            {
                return new StepResult(StepStatus.ProductNotFound);
            }

            step.Product = product;
            step.ProductId = product.Id;
        }

        if (dto.StepOrder != step.StepOrder
            && await _routines.StepOrderTakenAsync(routineId, dto.StepOrder, step.Id))
        {
            return new StepResult(StepStatus.OrderTaken);
        }

        dto.ApplyTo(step);
        await _routines.UpdateStepAsync(step);
        return new StepResult(StepStatus.Success, step.ToDto());
    }

    public async Task<bool> DeleteStepAsync(int routineId, int stepId)
    {
        var step = await _routines.GetStepAsync(routineId, stepId);
        if (step is null)
        {
            return false;
        }

        await _routines.DeleteStepAsync(step);
        return true;
    }
}
