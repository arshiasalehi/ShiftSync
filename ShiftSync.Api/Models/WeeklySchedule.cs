namespace ShiftSync.Api.Models;

public sealed class WeeklySchedule
{
    public int Id { get; set; }

    public int BusinessId { get; set; }
    public Business Business { get; set; } = null!;

    public DateTime WeekStartDate { get; set; }
    public bool IsPublished { get; set; }

    public ICollection<Shift> Shifts { get; set; } = [];
}
