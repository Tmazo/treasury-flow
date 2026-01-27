using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TreasuryFlow.Api.Auth.Requests;
using TreasuryFlow.Application.Auth.Services.Interfaces;

namespace TreasuryFlow.Api.Auth;

[AllowAnonymous]
[ApiController]
[Route("api/[controller]")]
public class AuthController(
    IAuthService authService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody]RegisterRequest request)
    {
        await authService.Register(request.ToInput());
        return Ok();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var token = await authService.Login(request.ToInput());
        return Ok(new { token });
    }
}