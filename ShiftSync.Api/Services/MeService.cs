using ShiftSync.Api.Infrastructure;
using ShiftSync.Api.Models;
using ShiftSync.Api.Repositories.Interfaces;
using ShiftSync.Api.Services.Interfaces;
using ShiftSync.Api.Utilities;
using ShiftSync.Shared.Contracts;

namespace ShiftSync.Api.Services;

public sealed class MeService(
    IUserRepository userRepository,
    IAvailabilityRepository availabilityRepository,
    IScheduleRepository scheduleRepository) : IMeService
{
    public async Task<List<ShiftDto>> GetShiftsAsync(int userId, DateTime? weekStartDate = null, CancellationToken cancellationToken = default)
    {
        await EnsureEmployeeAsync(userId, cancellationToken);

        DateTime? normalized = null;
        if (weekStartDate.HasValue)
        {
            normalized = WeekDateHelper.NormalizeWeekStart(weekStartDate.Value);
        }

        var shifts = await scheduleRepository.GetEmployeeShiftsAsync(userId, normalized, false, cancellationToken);
        return shifts.Select(MapShift).ToList();
    }

    public async Task<PayrollEstimateDto> GetPayrollAsync(int userId, DateTime? weekStartDate = null, CancellationToken cancellationToken = default)
    {
        var employee = await EnsureEmployeeAsync(userId, cancellationToken);
        var weekStart = WeekDateHelper.NormalizeWeekStart(weekStartDate ?? DateTime.UtcNow.Date);

        var shifts = await scheduleRepository.GetEmployeeShiftsAsync(userId, weekStart, true, cancellationToken);
        var totalHours = shifts.Sum(x => (decimal)(x.EndTime - x.StartTime).TotalHours);
        var hourlyRate = employee.EmployeeProfile?.HourlyRate ?? 0;

        return new PayrollEstimateDto
        {
            EmployeeId = employee.Id,
            EmployeeName = employee.FullName,
            WeekStartDate = weekStart,
            HourlyRate = hourlyRate,
            TotalHours = totalHours,
            TotalEstimatedPay = totalHours * hourlyRate
        };
    }

    public async Task<List<AvailabilityDto>> GetAvailabilityAsync(int userId, CancellationToken cancellationToken = default)
    {
        var employee = await EnsureEmployeeAsync(userId, cancellationToken);
        var rows = await availabilityRepository.GetForEmployeeAsync(userId, cancellationToken);

        return rows.Select(x => MapAvailability(x, employee.FullName)).ToList();
    }

    public async Task<AvailabilityDto> AddAvailabilityAsync(int userId, UpsertAvailabilityRequest request, CancellationToken cancellationToken = default)
    {
        var employee = await EnsureEmployeeAsync(userId, cancellationToken);
        ValidateAvailabilityInput(request);

        var existing = await availabilityRepository.GetForEmployeeAsync(userId, cancellationToken);
        EnsureNoOverlap(existing, request);

        var availability = new Availability
        {
            EmployeeId = userId,
            DayOfWeek = request.DayOfWeek,
            StartTime = request.StartTime,
            EndTime = request.EndTime
        };

        await availabilityRepository.AddAsync(availability, cancellationToken);
        await availabilityRepository.SaveChangesAsync(cancellationToken);

        return MapAvailability(availability, employee.FullName);
    }

    public async Task<AvailabilityDto> UpdateAvailabilityAsync(
        int userId,
        int availabilityId,
        UpsertAvailabilityRequest request,
        CancellationToken cancellationToken = default)
    {
        var employee = await EnsureEmployeeAsync(userId, cancellationToken);
        ValidateAvailabilityInput(request);

        var existingRow = await availabilityRepository.GetByIdAsync(availabilityId, cancellationToken);
        if (existingRow is null || existingRow.EmployeeId != userId)
        {
            throw new ApiException("Availability slot was not found.", StatusCodes.Status404NotFound);
        }

        var allRows = await availabilityRepository.GetForEmployeeAsync(userId, cancellationToken);
        EnsureNoOverlap(allRows, request, availabilityId);

        existingRow.DayOfWeek = request.DayOfWeek;
        existingRow.StartTime = request.StartTime;
        existingRow.EndTime = request.EndTime;

        await availabilityRepository.SaveChangesAsync(cancellationToken);
        return MapAvailability(existingRow, employee.FullName);
    }

    public async Task DeleteAvailabilityAsync(int userId, int availabilityId, CancellationToken cancellationToken = default)
    {
        await EnsureEmployeeAsync(userId, cancellationToken);

        var existingRow = await availabilityRepository.GetByIdAsync(availabilityId, cancellationToken);
        if (existingRow is null || existingRow.EmployeeId != userId)
        {
            throw new ApiException("Availability slot was not found.", StatusCodes.Status404NotFound);
        }

        availabilityRepository.Remove(existingRow);
        await availabilityRepository.SaveChangesAsync(cancellationToken);
    }

    private async Task<User> EnsureEmployeeAsync(int userId, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(userId, cancellationToken)
                   ?? throw new ApiException("User was not found.", StatusCodes.Status404NotFound);

        if (user.Role != UserRoles.Employee)
        {
            throw new ApiException("Only employees can access this endpoint.", StatusCodes.Status403Forbidden);
        }

        return user;
    }

    private static AvailabilityDto MapAvailability(Availability row, string employeeName)
        => new()
        {
            Id = row.Id,
            EmployeeId = row.EmployeeId,
            EmployeeName = employeeName,
            DayOfWeek = row.DayOfWeek,
            StartTime = row.StartTime,
            EndTime = row.EndTime
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

    private static void ValidateAvailabilityInput(UpsertAvailabilityRequest request)
    {
        if (request.DayOfWeek < 0 || request.DayOfWeek > 6)
        {
            throw new ApiException("DayOfWeek must be between 0 and 6.");
        }

        if (request.EndTime <= request.StartTime)
        {
            throw new ApiException("End time must be after start time.");
        }
    }

    private static void EnsureNoOverlap(IEnumerable<Availability> rows, UpsertAvailabilityRequest request, int? ignoreAvailabilityId = null)
    {
        var overlap = rows.Any(x =>
            x.Id != ignoreAvailabilityId
            && x.DayOfWeek == request.DayOfWeek
            && request.StartTime < x.EndTime
            && request.EndTime > x.StartTime);

        if (overlap)
        {
            throw new ApiException("Availability slots cannot overlap.");
        }
    }
}
