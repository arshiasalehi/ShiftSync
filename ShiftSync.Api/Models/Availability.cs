namespace ShiftSync.Api.Models;

public sealed class Availability
{
    public int Id { get; set; }

    public int EmployeeId { get; set; }
    public User Employee { get; set; } = null!;

    public int DayOfWeek { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
}
