using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShiftSync.Api.Infrastructure;
using ShiftSync.Api.Services.Interfaces;
using ShiftSync.Shared.Contracts;

namespace ShiftSync.Api.Controllers;

[ApiController]
[Authorize(Roles = UserRoles.Admin)]
[Route("api/availability")]
public sealed class AvailabilityController(IAdminService adminService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<AvailabilityDto>>> GetAvailability(CancellationToken cancellationToken)
        => Ok(await adminService.GetAvailabilityAsync(User.GetBusinessId(), cancellationToken));
}
