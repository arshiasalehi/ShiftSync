using System.Collections.ObjectModel;
using System.Windows.Input;
using ShiftSync.Mobile.Services;
using ShiftSync.Shared.Contracts;

namespace ShiftSync.Mobile.ViewModels;

public sealed class ShiftsViewModel(ApiClient apiClient, SessionState sessionState) : BaseViewModel
{
    private DateTime _weekStartDate = NormalizeWeekStart(DateTime.Today);

    public ObservableCollection<ShiftDto> Shifts { get; } = [];

    public DateTime WeekStartDate
    {
        get => _weekStartDate;
        set
        {
            if (SetProperty(ref _weekStartDate, NormalizeWeekStart(value)))
            {
                OnPropertyChanged(nameof(TotalHours));
            }
        }
    }

    public decimal TotalHours => Shifts.Sum(x => x.Hours);

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
            var shifts = await apiClient.GetMyShiftsAsync(WeekStartDate);
            Shifts.Clear();

            foreach (var shift in shifts.OrderBy(x => x.ShiftDate).ThenBy(x => x.StartTime))
            {
                Shifts.Add(shift);
            }

            OnPropertyChanged(nameof(TotalHours));
        }, "Shifts loaded.");
    }
}
