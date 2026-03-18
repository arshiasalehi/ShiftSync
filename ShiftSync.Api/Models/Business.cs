namespace ShiftSync.Api.Models;

public sealed class Business
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string CompanyCode { get; set; } = string.Empty;

    public ICollection<User> Users { get; set; } = [];
    public ICollection<WeeklySchedule> WeeklySchedules { get; set; } = [];
}
