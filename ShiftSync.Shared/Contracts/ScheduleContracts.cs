namespace ShiftSync.Shared.Contracts;

public sealed class CreateScheduleRequest
{
    public DateTime WeekStartDate { get; set; }
    public bool IsPublished { get; set; }
}

public sealed class CreateShiftRequest
{
    public int WeeklyScheduleId { get; set; }
    public int EmployeeId { get; set; }
    public int RoleTypeId { get; set; }
    public DateTime ShiftDate { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
}

public sealed class ShiftDto
{
    public int Id { get; set; }
    public int WeeklyScheduleId { get; set; }
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public int RoleTypeId { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public DateTime ShiftDate { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public decimal Hours { get; set; }
}

public sealed class WeeklyScheduleDto
{
    public int Id { get; set; }
    public int BusinessId { get; set; }
    public DateTime WeekStartDate { get; set; }
    public bool IsPublished { get; set; }
    public List<ShiftDto> Shifts { get; set; } = [];
}
