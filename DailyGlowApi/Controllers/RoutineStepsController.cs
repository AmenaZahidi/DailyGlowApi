using DailyGlowApi.Dtos;
using DailyGlowApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DailyGlowApi.Controllers;

// Handles the steps inside a routine: /api/routines/{routineId}/steps
[ApiController]
[Route("api/routines/{routineId:int}/steps")]
public class RoutineStepsController : ControllerBase
{
    private readonly IRoutineService _service;

    public RoutineStepsController(IRoutineService service)
    {
        _service = service;
    }

    // GET api/routines/3/steps
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<RoutineStepDto>>> GetAll(int routineId)
    {
        var steps = await _service.GetStepsAsync(routineId);
        if (steps is null)
        {
            return NotFound();
        }

        return Ok(steps);
    }

    // POST api/routines/3/steps
    [Authorize]
    [HttpPost]
    public async Task<ActionResult<RoutineStepDto>> Create(int routineId, CreateStepDto dto)
    {
        var result = await _service.AddStepAsync(routineId, dto);
        if (result.Status != StepStatus.Success)
        {
            return ToError(result.Status);
        }

        // The Location header points to the list of steps in this routine
        return CreatedAtAction(nameof(GetAll), new { routineId }, result.Step);
    }

    // PUT api/routines/3/steps/7
    [Authorize]
    [HttpPut("{stepId:int}")]
    public async Task<IActionResult> Update(int routineId, int stepId, UpdateStepDto dto)
    {
        var result = await _service.UpdateStepAsync(routineId, stepId, dto);
        if (result.Status != StepStatus.Success)
        {
            return ToError(result.Status);
        }

        return NoContent();
    }

    // DELETE api/routines/3/steps/7
    [Authorize]
    [HttpDelete("{stepId:int}")]
    public async Task<IActionResult> Delete(int routineId, int stepId)
    {
        var deleted = await _service.DeleteStepAsync(routineId, stepId);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }

    // Turns a failed step result into the right HTTP error
    private ActionResult ToError(StepStatus status)
    {
        return status switch
        {
            StepStatus.ProductNotFound => Problem(
                title: "Product does not exist",
                detail: "The ProductId does not match any product.",
                statusCode: StatusCodes.Status400BadRequest),
            StepStatus.OrderTaken => Problem(
                title: "Step order already used",
                detail: "Another step in this routine already has this StepOrder.",
                statusCode: StatusCodes.Status409Conflict),
            _ => NotFound()
        };
    }
}
