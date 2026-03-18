using System.Windows.Input;
using ShiftSync.Mobile.Services;
using ShiftSync.Shared.Contracts;

namespace ShiftSync.Mobile.ViewModels;

public sealed class PayrollViewModel(ApiClient apiClient, SessionState sessionState) : BaseViewModel
{
    private DateTime _weekStartDate = NormalizeWeekStart(DateTime.Today);
    private PayrollEstimateDto? _payroll;

    public DateTime WeekStartDate
    {
        get => _weekStartDate;
        set => SetProperty(ref _weekStartDate, NormalizeWeekStart(value));
    }

    public PayrollEstimateDto? Payroll
    {
        get => _payroll;
        set
        {
            if (SetProperty(ref _payroll, value))
            {
                OnPropertyChanged(nameof(HasPayroll));
            }
        }
    }

    public bool HasPayroll => Payroll is not null;

    public ICommand LoadCommand => new Command(async () => await LoadAsync());

    public async Task LoadAsync()
    {
        if (!sessionState.IsEmployee)
        {
            HasError = true;
            StatusMessage = "Employee login is required.";
            return;
        }

        await RunAsync(async () =>
        {
            Payroll = await apiClient.GetMyPayrollAsync(WeekStartDate);
        }, "Payroll loaded.");
    }
}
