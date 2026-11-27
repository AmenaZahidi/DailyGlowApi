using DailyGlowApi.Dtos;
using DailyGlowApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace DailyGlowApi.Controllers;

// Register and login (/api/auth). Anyone can call it.
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _service;

    public AuthController(IAuthService service)
    {
        _service = service;
    }

    // POST api/auth/register
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        var result = await _service.RegisterAsync(dto);

        if (result == RegisterResult.EmailTaken)
        {
            return Problem(
                title: "Email already registered",
                detail: "An account with this email already exists.",
                statusCode: StatusCodes.Status409Conflict);
        }

        return StatusCode(StatusCodes.Status201Created);
    }

    // POST api/auth/login
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginDto dto)
    {
        var response = await _service.LoginAsync(dto);
        if (response is null)
        {
            // Same message for a wrong email and a wrong password
            return Problem(
                title: "Login failed",
                detail: "The email or the password is wrong.",
                statusCode: StatusCodes.Status401Unauthorized);
        }

        return Ok(response);
    }
}
