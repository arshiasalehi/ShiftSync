using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShiftSync.Api.Infrastructure;
using ShiftSync.Api.Services.Interfaces;
using ShiftSync.Shared.Contracts;

namespace ShiftSync.Api.Controllers;

[ApiController]
[Authorize(Roles = UserRoles.Admin)]
[Route("api/schedules")]
public sealed class SchedulesController(IAdminService adminService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<WeeklyScheduleDto>> CreateSchedule(CreateScheduleRequest request, CancellationToken cancellationToken)
        => Ok(await adminService.CreateScheduleAsync(User.GetBusinessId(), request, cancellationToken));

    [HttpGet("{weekStart}")]
    public async Task<ActionResult<WeeklyScheduleDto>> GetSchedule([FromRoute] DateTime weekStart, CancellationToken cancellationToken)
        => Ok(await adminService.GetScheduleAsync(User.GetBusinessId(), weekStart, cancellationToken));
}
