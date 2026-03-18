using ShiftSync.Api.Models;

namespace ShiftSync.Api.Repositories.Interfaces;

public interface IScheduleRepository
{
    Task<WeeklySchedule?> GetByIdAsync(int scheduleId, CancellationToken cancellationToken = default);
    Task<WeeklySchedule?> GetByBusinessAndWeekAsync(int businessId, DateTime weekStartDate, CancellationToken cancellationToken = default);
    Task AddScheduleAsync(WeeklySchedule schedule, CancellationToken cancellationToken = default);
    Task AddShiftAsync(Shift shift, CancellationToken cancellationToken = default);
    Task<List<Shift>> GetEmployeeWeekShiftsAsync(int employeeId, DateTime weekStartDate, CancellationToken cancellationToken = default);
    Task<List<Shift>> GetEmployeeShiftsAsync(int employeeId, DateTime? weekStartDate = null, bool publishedOnly = false, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
