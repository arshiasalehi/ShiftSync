using ShiftSync.Api.Infrastructure;
using ShiftSync.Api.Models;
using ShiftSync.Api.Repositories.Interfaces;
using ShiftSync.Api.Services.Interfaces;
using ShiftSync.Api.Utilities;
using ShiftSync.Shared.Contracts;

namespace ShiftSync.Api.Services;

public sealed class AdminService(
    IUserRepository userRepository,
    IRoleRepository roleRepository,
    IAvailabilityRepository availabilityRepository,
    IScheduleRepository scheduleRepository) : IAdminService
{
    public async Task<List<EmployeeDto>> GetEmployeesAsync(int businessId, CancellationToken cancellationToken = default)
    {
        var employees = await userRepository.GetEmployeesByBusinessAsync(businessId, cancellationToken);
        return employees.Select(MapEmployee).ToList();
    }

    public async Task<EmployeeDto> UpdatePayRateAsync(int businessId, int employeeId, UpdatePayRateRequest request, CancellationToken cancellationToken = default)
    {
        if (request.HourlyRate < 0)
        {
            throw new ApiException("Hourly rate cannot be negative.");
        }

        if (request.MaxWeeklyHours <= 0 || request.MaxWeeklyHours > 168)
        {
            throw new ApiException("MaxWeeklyHours must be between 1 and 168.");
        }

        var employee = await GetEmployeeForBusinessAsync(businessId, employeeId, cancellationToken);
        employee.EmployeeProfile ??= new EmployeeProfile { UserId = employee.Id };
        employee.EmployeeProfile.HourlyRate = request.HourlyRate;
        employee.EmployeeProfile.MaxWeeklyHours = request.MaxWeeklyHours;

        await userRepository.SaveChangesAsync(cancellationToken);
        return MapEmployee(employee);
    }

    public async Task<EmployeeDto> UpdateRolesAsync(int businessId, int employeeId, UpdateEmployeeRolesRequest request, CancellationToken cancellationToken = default)
    {
        var roleIds = request.RoleTypeIds.Distinct().ToList();
        if (roleIds.Count == 0)
        {
            throw new ApiException("At least one role must be selected.");
        }

        var roles = await roleRepository.GetByIdsAsync(roleIds, cancellationToken);
        if (roles.Count != roleIds.Count)
        {
            throw new ApiException("One or more role IDs are invalid.");
        }

        var employee = await GetEmployeeForBusinessAsync(businessId, employeeId, cancellationToken);

        employee.EmployeeRoles.Clear();
        foreach (var role in roles)
        {
            employee.EmployeeRoles.Add(new EmployeeRole
            {
                EmployeeId = employee.Id,
                RoleTypeId = role.Id
            });
        }

        await userRepository.SaveChangesAsync(cancellationToken);
        return MapEmployee(employee);
    }

    public async Task<List<RoleTypeDto>> GetRoleTypesAsync(CancellationToken cancellationToken = default)
    {
        var roles = await roleRepository.GetAllAsync(cancellationToken);
        return roles.Select(x => new RoleTypeDto { Id = x.Id, Name = x.Name }).ToList();
    }

    public async Task<List<AvailabilityDto>> GetAvailabilityAsync(int businessId, CancellationToken cancellationToken = default)
    {
        var rows = await availabilityRepository.GetForBusinessAsync(businessId, cancellationToken);

        return rows.Select(x => new AvailabilityDto
        {
            Id = x.Id,
            EmployeeId = x.EmployeeId,
            EmployeeName = x.Employee.FullName,
            DayOfWeek = x.DayOfWeek,
            StartTime = x.StartTime,
            EndTime = x.EndTime
        }).ToList();
    }

    public async Task<WeeklyScheduleDto> CreateScheduleAsync(int businessId, CreateScheduleRequest request, CancellationToken cancellationToken = default)
    {
        var weekStart = WeekDateHelper.NormalizeWeekStart(request.WeekStartDate);

        var existing = await scheduleRepository.GetByBusinessAndWeekAsync(businessId, weekStart, cancellationToken);
        if (existing is not null)
        {
            throw new ApiException("A schedule for that week already exists.", StatusCodes.Status409Conflict);
        }

        var schedule = new WeeklySchedule
        {
            BusinessId = businessId,
            WeekStartDate = weekStart,
            IsPublished = request.IsPublished
        };

        await scheduleRepository.AddScheduleAsync(schedule, cancellationToken);
        await scheduleRepository.SaveChangesAsync(cancellationToken);

        return new WeeklyScheduleDto
        {
            Id = schedule.Id,
            BusinessId = schedule.BusinessId,
            WeekStartDate = schedule.WeekStartDate,
            IsPublished = schedule.IsPublished
        };
    }

    public async Task<WeeklyScheduleDto> GetScheduleAsync(int businessId, DateTime weekStartDate, CancellationToken cancellationToken = default)
    {
        var weekStart = WeekDateHelper.NormalizeWeekStart(weekStartDate);

        var schedule = await scheduleRepository.GetByBusinessAndWeekAsync(businessId, weekStart, cancellationToken)
            ?? throw new ApiException("Schedule not found.", StatusCodes.Status404NotFound);

        return MapSchedule(schedule);
    }

    public async Task<ShiftDto> CreateShiftAsync(int businessId, CreateShiftRequest request, CancellationToken cancellationToken = default)
    {
        if (request.EndTime <= request.StartTime)
        {
            throw new ApiException("Shift end time must be after start time.");
        }

        var shiftDuration = (decimal)(request.EndTime - request.StartTime).TotalHours;
        if (shiftDuration < 4)
        {
            throw new ApiException("Minimum shift length is 4 hours.");
        }

        var schedule = await scheduleRepository.GetByIdAsync(request.WeeklyScheduleId, cancellationToken)
            ?? throw new ApiException("Schedule not found.", StatusCodes.Status404NotFound);

        if (schedule.BusinessId != businessId)
        {
            throw new ApiException("Schedule does not belong to this business.", StatusCodes.Status403Forbidden);
        }

        var shiftDate = request.ShiftDate.Date;
        if (shiftDate < schedule.WeekStartDate || shiftDate >= schedule.WeekStartDate.AddDays(7))
        {
            throw new ApiException("Shift date must be inside the selected schedule week.");
        }

        var employee = await GetEmployeeForBusinessAsync(businessId, request.EmployeeId, cancellationToken);
        var role = await roleRepository.GetByIdAsync(request.RoleTypeId, cancellationToken)
            ?? throw new ApiException("Role type was not found.", StatusCodes.Status404NotFound);

        if (!employee.EmployeeRoles.Any(x => x.RoleTypeId == role.Id))
        {
            throw new ApiException("Employee is not assigned to the requested role.");
        }

        var availability = await availabilityRepository.GetForEmployeeAsync(employee.Id, cancellationToken);
        var shiftDay = (int)shiftDate.DayOfWeek;

        // A shift is valid only when it is fully enclosed by at least one availability slot.
        var fitsAvailability = availability.Any(x =>
            x.DayOfWeek == shiftDay
            && request.StartTime >= x.StartTime
            && request.EndTime <= x.EndTime);

        if (!fitsAvailability)
        {
            throw new ApiException("Shift must be inside employee availability.");
        }

        var weekStart = WeekDateHelper.NormalizeWeekStart(schedule.WeekStartDate);
        var weeklyShifts = await scheduleRepository.GetEmployeeWeekShiftsAsync(employee.Id, weekStart, cancellationToken);

        if (weeklyShifts.Any(x => x.ShiftDate.Date == shiftDate))
        {
            throw new ApiException("An employee can only have one shift per day.");
        }

        var weeklyHours = weeklyShifts.Sum(x => (decimal)(x.EndTime - x.StartTime).TotalHours);
        var maxAllowed = employee.EmployeeProfile?.MaxWeeklyHours ?? 0;
        if (weeklyHours + shiftDuration > maxAllowed)
        {
            throw new ApiException("Shift exceeds max weekly hours for this employee.");
        }

        var shift = new Shift
        {
            WeeklyScheduleId = schedule.Id,
            EmployeeId = employee.Id,
            RoleTypeId = role.Id,
            ShiftDate = shiftDate,
            StartTime = request.StartTime,
            EndTime = request.EndTime
        };

        await scheduleRepository.AddShiftAsync(shift, cancellationToken);
        await scheduleRepository.SaveChangesAsync(cancellationToken);

        shift.Employee = employee;
        shift.RoleType = role;

        return MapShift(shift);
    }

    private async Task<User> GetEmployeeForBusinessAsync(int businessId, int employeeId, CancellationToken cancellationToken)
        => await userRepository.GetEmployeeByIdAsync(businessId, employeeId, cancellationToken)
           ?? throw new ApiException("Employee not found.", StatusCodes.Status404NotFound);

    private static EmployeeDto MapEmployee(User employee)
        => new()
        {
            Id = employee.Id,
            FullName = employee.FullName,
            Email = employee.Email,
            HourlyRate = employee.EmployeeProfile?.HourlyRate ?? 0,
            MaxWeeklyHours = employee.EmployeeProfile?.MaxWeeklyHours ?? 0,
            Roles = employee.EmployeeRoles
                .Select(x => new RoleTypeDto
                {
                    Id = x.RoleTypeId,
                    Name = x.RoleType.Name
                })
                .OrderBy(x => x.Name)
                .ToList()
        };

    private static WeeklyScheduleDto MapSchedule(WeeklySchedule schedule)
        => new()
        {
            Id = schedule.Id,
            BusinessId = schedule.BusinessId,
            WeekStartDate = schedule.WeekStartDate,
            IsPublished = schedule.IsPublished,
            Shifts = schedule.Shifts
                .OrderBy(x => x.ShiftDate)
                .ThenBy(x => x.StartTime)
                .Select(MapShift)
                .ToList()
        };

    private static ShiftDto MapShift(Shift shift)
        => new()
        {
            Id = shift.Id,
            WeeklyScheduleId = shift.WeeklyScheduleId,
            EmployeeId = shift.EmployeeId,
            EmployeeName = shift.Employee.FullName,
            RoleTypeId = shift.RoleTypeId,
            RoleName = shift.RoleType.Name,
            ShiftDate = shift.ShiftDate,
            StartTime = shift.StartTime,
            EndTime = shift.EndTime,
            Hours = (decimal)(shift.EndTime - shift.StartTime).TotalHours
        };
}
