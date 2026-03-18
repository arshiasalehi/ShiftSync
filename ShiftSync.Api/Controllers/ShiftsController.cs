using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShiftSync.Api.Infrastructure;
using ShiftSync.Api.Services.Interfaces;
using ShiftSync.Shared.Contracts;

namespace ShiftSync.Api.Controllers;

[ApiController]
[Authorize(Roles = UserRoles.Admin)]
[Route("api/shifts")]
public sealed class ShiftsController(IAdminService adminService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<ShiftDto>> CreateShift(CreateShiftRequest request, CancellationToken cancellationToken)
        => Ok(await adminService.CreateShiftAsync(User.GetBusinessId(), request, cancellationToken));
}
