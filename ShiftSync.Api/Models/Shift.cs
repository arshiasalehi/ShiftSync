namespace ShiftSync.Api.Models;

public sealed class Shift
{
    public int Id { get; set; }

    public int WeeklyScheduleId { get; set; }
    public WeeklySchedule WeeklySchedule { get; set; } = null!;

    public int EmployeeId { get; set; }
    public User Employee { get; set; } = null!;

    public int RoleTypeId { get; set; }
    public RoleType RoleType { get; set; } = null!;

    public DateTime ShiftDate { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
}
