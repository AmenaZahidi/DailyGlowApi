using DailyGlowApi.Dtos;
using DailyGlowApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace DailyGlowApi.Controllers;

// Handles the clash check of a routine: /api/routines/{routineId}/clashes
// Anyone can call it, so there is no [Authorize].
[ApiController]
[Route("api/routines/{routineId:int}/clashes")]
public class RoutineClashesController : ControllerBase
{
    private readonly IRoutineService _service;

    public RoutineClashesController(IRoutineService service)
    {
        _service = service;
    }

    // GET api/routines/3/clashes
    // Lists the products in the routine that should not be used together
    [HttpGet]
    public async Task<ActionResult<ClashReportDto>> Get(int routineId)
    {
        var report = await _service.CheckClashesAsync(routineId);
        if (report is null)
        {
            return NotFound();
        }

        return Ok(report);
    }
}
