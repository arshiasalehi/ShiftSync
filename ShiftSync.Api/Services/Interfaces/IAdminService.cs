using ShiftSync.Shared.Contracts;

namespace ShiftSync.Api.Services.Interfaces;

public interface IAdminService
{
    Task<List<EmployeeDto>> GetEmployeesAsync(int businessId, CancellationToken cancellationToken = default);
    Task<EmployeeDto> UpdatePayRateAsync(int businessId, int employeeId, UpdatePayRateRequest request, CancellationToken cancellationToken = default);
    Task<EmployeeDto> UpdateRolesAsync(int businessId, int employeeId, UpdateEmployeeRolesRequest request, CancellationToken cancellationToken = default);
    Task<List<RoleTypeDto>> GetRoleTypesAsync(CancellationToken cancellationToken = default);
    Task<List<AvailabilityDto>> GetAvailabilityAsync(int businessId, CancellationToken cancellationToken = default);
    Task<WeeklyScheduleDto> CreateScheduleAsync(int businessId, CreateScheduleRequest request, CancellationToken cancellationToken = default);
    Task<WeeklyScheduleDto> GetScheduleAsync(int businessId, DateTime weekStartDate, CancellationToken cancellationToken = default);
    Task<ShiftDto> CreateShiftAsync(int businessId, CreateShiftRequest request, CancellationToken cancellationToken = default);
}
