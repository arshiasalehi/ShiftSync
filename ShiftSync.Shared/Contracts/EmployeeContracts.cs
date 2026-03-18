namespace ShiftSync.Shared.Contracts;

public sealed class RoleTypeDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public sealed class EmployeeDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public decimal HourlyRate { get; set; }
    public int MaxWeeklyHours { get; set; }
    public List<RoleTypeDto> Roles { get; set; } = [];
}

public sealed class UpdatePayRateRequest
{
    public decimal HourlyRate { get; set; }
    public int MaxWeeklyHours { get; set; }
}

public sealed class UpdateEmployeeRolesRequest
{
    public List<int> RoleTypeIds { get; set; } = [];
}
