using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShiftSync.Api.Infrastructure;
using ShiftSync.Api.Services.Interfaces;
using ShiftSync.Shared.Contracts;

namespace ShiftSync.Api.Controllers;

[ApiController]
[Authorize(Roles = UserRoles.Admin)]
[Route("api/employees")]
public sealed class EmployeesController(IAdminService adminService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<EmployeeDto>>> GetEmployees(CancellationToken cancellationToken)
        => Ok(await adminService.GetEmployeesAsync(User.GetBusinessId(), cancellationToken));

    [HttpGet("role-types")]
    public async Task<ActionResult<List<RoleTypeDto>>> GetRoleTypes(CancellationToken cancellationToken)
        => Ok(await adminService.GetRoleTypesAsync(cancellationToken));

    [HttpPut("{id:int}/payrate")]
    public async Task<ActionResult<EmployeeDto>> UpdatePayRate(int id, UpdatePayRateRequest request, CancellationToken cancellationToken)
        => Ok(await adminService.UpdatePayRateAsync(User.GetBusinessId(), id, request, cancellationToken));

    [HttpPut("{id:int}/roles")]
    public async Task<ActionResult<EmployeeDto>> UpdateRoles(int id, UpdateEmployeeRolesRequest request, CancellationToken cancellationToken)
        => Ok(await adminService.UpdateRolesAsync(User.GetBusinessId(), id, request, cancellationToken));
}
