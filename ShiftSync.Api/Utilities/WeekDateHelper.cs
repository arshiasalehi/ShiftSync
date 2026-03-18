namespace ShiftSync.Api.Utilities;

public static class WeekDateHelper
{
    // The app uses Monday as the canonical week start for scheduling/payroll queries.
    public static DateTime NormalizeWeekStart(DateTime date)
    {
        var normalized = date.Date;
        var offset = ((int)normalized.DayOfWeek + 6) % 7;
        return normalized.AddDays(-offset);
    }
}
