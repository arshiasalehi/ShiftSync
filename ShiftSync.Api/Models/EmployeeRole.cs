namespace ShiftSync.Api.Models;

public sealed class EmployeeRole
{
    public int Id { get; set; }

    public int EmployeeId { get; set; }
    public User Employee { get; set; } = null!;

    public int RoleTypeId { get; set; }
    public RoleType RoleType { get; set; } = null!;
}
