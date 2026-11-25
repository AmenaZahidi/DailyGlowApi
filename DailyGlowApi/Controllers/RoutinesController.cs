using DailyGlowApi.Dtos;
using DailyGlowApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace DailyGlowApi.Controllers;

// Handles the /api/routines endpoints.
// The steps are handled in RoutineStepsController.
[ApiController]
[Route("api/[controller]")]
public class RoutinesController : ControllerBase
{
    private readonly IRoutineService _service;

    public RoutinesController(IRoutineService service)
    {
        _service = service;
    }

    // GET api/routines
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<RoutineSummaryDto>>> GetAll()
    {
        var routines = await _service.GetAllAsync();
        return Ok(routines);
    }

    // GET api/routines/3
    [HttpGet("{id:int}")]
    public async Task<ActionResult<RoutineDetailDto>> GetById(int id)
    {
        var routine = await _service.GetByIdAsync(id);
        if (routine is null)
        {
            return NotFound();
        }

        return Ok(routine);
    }

    // POST api/routines
    [HttpPost]
    public async Task<ActionResult<RoutineDetailDto>> Create(CreateRoutineDto dto)
    {
        var created = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    // PUT api/routines/3
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateRoutineDto dto)
    {
        var updated = await _service.UpdateAsync(id, dto);
        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

    // DELETE api/routines/3 (its steps are deleted too)
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
