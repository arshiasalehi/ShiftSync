using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShiftSync.Api.Infrastructure;
using ShiftSync.Api.Services.Interfaces;
using ShiftSync.Shared.Contracts;

namespace ShiftSync.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/me")]
public sealed class MeController(IMeService meService) : ControllerBase
{
    [HttpGet("shifts")]
    public async Task<ActionResult<List<ShiftDto>>> GetShifts([FromQuery] DateTime? weekStart, CancellationToken cancellationToken)
        => Ok(await meService.GetShiftsAsync(User.GetUserId(), weekStart, cancellationToken));

    [HttpGet("payroll")]
    public async Task<ActionResult<PayrollEstimateDto>> GetPayroll([FromQuery] DateTime? weekStart, CancellationToken cancellationToken)
        => Ok(await meService.GetPayrollAsync(User.GetUserId(), weekStart, cancellationToken));

    [HttpGet("availability")]
    public async Task<ActionResult<List<AvailabilityDto>>> GetAvailability(CancellationToken cancellationToken)
        => Ok(await meService.GetAvailabilityAsync(User.GetUserId(), cancellationToken));

    [HttpPost("availability")]
    public async Task<ActionResult<AvailabilityDto>> AddAvailability(UpsertAvailabilityRequest request, CancellationToken cancellationToken)
        => Ok(await meService.AddAvailabilityAsync(User.GetUserId(), request, cancellationToken));

    [HttpPut("availability/{id:int}")]
    public async Task<ActionResult<AvailabilityDto>> UpdateAvailability(int id, UpsertAvailabilityRequest request, CancellationToken cancellationToken)
        => Ok(await meService.UpdateAvailabilityAsync(User.GetUserId(), id, request, cancellationToken));

    [HttpDelete("availability/{id:int}")]
    public async Task<IActionResult> DeleteAvailability(int id, CancellationToken cancellationToken)
    {
        await meService.DeleteAvailabilityAsync(User.GetUserId(), id, cancellationToken);
        return NoContent();
    }
}
