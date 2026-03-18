namespace ShiftSync.Api.Models;

public sealed class RoleType
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public ICollection<EmployeeRole> EmployeeRoles { get; set; } = [];
    public ICollection<Shift> Shifts { get; set; } = [];
}
