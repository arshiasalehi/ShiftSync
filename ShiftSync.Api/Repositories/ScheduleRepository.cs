using Microsoft.EntityFrameworkCore;
using ShiftSync.Api.Data;
using ShiftSync.Api.Models;
using ShiftSync.Api.Repositories.Interfaces;

namespace ShiftSync.Api.Repositories;

public sealed class ScheduleRepository(ShiftSyncDbContext dbContext) : IScheduleRepository
{
    public Task<WeeklySchedule?> GetByIdAsync(int scheduleId, CancellationToken cancellationToken = default)
        => dbContext.WeeklySchedules
            .Include(x => x.Shifts)
            .ThenInclude(x => x.Employee)
            .Include(x => x.Shifts)
            .ThenInclude(x => x.RoleType)
            .FirstOrDefaultAsync(x => x.Id == scheduleId, cancellationToken);

    public Task<WeeklySchedule?> GetByBusinessAndWeekAsync(int businessId, DateTime weekStartDate, CancellationToken cancellationToken = default)
        => dbContext.WeeklySchedules
            .Include(x => x.Shifts)
            .ThenInclude(x => x.Employee)
            .Include(x => x.Shifts)
            .ThenInclude(x => x.RoleType)
            .FirstOrDefaultAsync(x => x.BusinessId == businessId && x.WeekStartDate == weekStartDate, cancellationToken);

    public Task AddScheduleAsync(WeeklySchedule schedule, CancellationToken cancellationToken = default)
        => dbContext.WeeklySchedules.AddAsync(schedule, cancellationToken).AsTask();

    public Task AddShiftAsync(Shift shift, CancellationToken cancellationToken = default)
        => dbContext.Shifts.AddAsync(shift, cancellationToken).AsTask();

    public Task<List<Shift>> GetEmployeeWeekShiftsAsync(int employeeId, DateTime weekStartDate, CancellationToken cancellationToken = default)
    {
        var weekEnd = weekStartDate.Date.AddDays(7);
        return dbContext.Shifts
            .Include(x => x.WeeklySchedule)
            .Where(x => x.EmployeeId == employeeId && x.ShiftDate >= weekStartDate && x.ShiftDate < weekEnd)
            .ToListAsync(cancellationToken);
    }

    public Task<List<Shift>> GetEmployeeShiftsAsync(
        int employeeId,
        DateTime? weekStartDate = null,
        bool publishedOnly = false,
        CancellationToken cancellationToken = default)
    {
        var query = dbContext.Shifts
            .Include(x => x.Employee)
            .Include(x => x.RoleType)
            .Include(x => x.WeeklySchedule)
            .Where(x => x.EmployeeId == employeeId)
            .AsQueryable();

        if (publishedOnly)
        {
            query = query.Where(x => x.WeeklySchedule.IsPublished);
        }

        if (weekStartDate.HasValue)
        {
            var weekStart = weekStartDate.Value.Date;
            var weekEnd = weekStart.AddDays(7);
            query = query.Where(x => x.ShiftDate >= weekStart && x.ShiftDate < weekEnd);
        }

        return query
            .OrderBy(x => x.ShiftDate)
            .ThenBy(x => x.StartTime)
            .ToListAsync(cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => dbContext.SaveChangesAsync(cancellationToken);
}
