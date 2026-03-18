namespace ShiftSync.Api.Models;

public sealed class EmployeeProfile
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public decimal HourlyRate { get; set; }
    public int MaxWeeklyHours { get; set; }
}
