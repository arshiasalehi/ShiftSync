namespace ShiftSync.Shared.Contracts;

public sealed class PayrollEstimateDto
{
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public DateTime WeekStartDate { get; set; }
    public decimal TotalHours { get; set; }
    public decimal HourlyRate { get; set; }
    public decimal TotalEstimatedPay { get; set; }
}
