namespace ShiftSync.Api.Models;

public sealed class User
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;

    public int BusinessId { get; set; }
    public Business Business { get; set; } = null!;

    public EmployeeProfile? EmployeeProfile { get; set; }
    public ICollection<EmployeeRole> EmployeeRoles { get; set; } = [];
    public ICollection<Availability> Availabilities { get; set; } = [];
    public ICollection<Shift> Shifts { get; set; } = [];
}
