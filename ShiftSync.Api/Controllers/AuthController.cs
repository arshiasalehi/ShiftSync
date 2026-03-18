using Microsoft.AspNetCore.Mvc;
using ShiftSync.Api.Services.Interfaces;
using ShiftSync.Shared.Contracts;

namespace ShiftSync.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("register-admin")]
    public async Task<ActionResult<AuthResponse>> RegisterAdmin(RegisterAdminRequest request, CancellationToken cancellationToken)
        => Ok(await authService.RegisterAdminAsync(request, cancellationToken));

    [HttpPost("register-employee")]
    public async Task<ActionResult<AuthResponse>> RegisterEmployee(RegisterEmployeeRequest request, CancellationToken cancellationToken)
        => Ok(await authService.RegisterEmployeeAsync(request, cancellationToken));

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request, CancellationToken cancellationToken)
        => Ok(await authService.LoginAsync(request, cancellationToken));
}
