using System.Collections.ObjectModel;
using System.Windows.Input;
using ShiftSync.Mobile.Services;
using ShiftSync.Shared.Contracts;

namespace ShiftSync.Mobile.ViewModels;

public sealed class AvailabilityViewModel : BaseViewModel
{
    private readonly ApiClient _apiClient;
    private readonly SessionState _sessionState;

    private DayOption _newDay;
    private string _newStartTime = "09:00";
    private string _newEndTime = "13:00";

    private int? _editingAvailabilityId;
    private DayOption _editDay;
    private string _editStartTime = "09:00";
    private string _editEndTime = "13:00";

    public AvailabilityViewModel(ApiClient apiClient, SessionState sessionState)
    {
        _apiClient = apiClient;
        _sessionState = sessionState;

        DayOptions = CreateDayOptions();
        _newDay = DayOptions[1];
        _editDay = DayOptions[1];
    }

    public ObservableCollection<AvailabilityDto> Slots { get; } = [];
    public List<DayOption> DayOptions { get; }

    public DayOption NewDay
    {
        get => _newDay;
        set => SetProperty(ref _newDay, value);
    }

    public string NewStartTime
    {
        get => _newStartTime;
        set => SetProperty(ref _newStartTime, value);
    }

    public string NewEndTime
    {
        get => _newEndTime;
        set => SetProperty(ref _newEndTime, value);
    }

    public int? EditingAvailabilityId
    {
        get => _editingAvailabilityId;
        set
        {
            if (SetProperty(ref _editingAvailabilityId, value))
            {
                OnPropertyChanged(nameof(IsEditing));
            }
        }
    }

    public bool IsEditing => EditingAvailabilityId.HasValue;

    public DayOption EditDay
    {
        get => _editDay;
        set => SetProperty(ref _editDay, value);
    }

    public string EditStartTime
    {
        get => _editStartTime;
        set => SetProperty(ref _editStartTime, value);
    }

    public string EditEndTime
    {
        get => _editEndTime;
        set => SetProperty(ref _editEndTime, value);
    }

    public ICommand LoadCommand => new Command(async () => await LoadAsync());
    public ICommand AddCommand => new Command(async () => await AddAsync());
    public ICommand BeginEditCommand => new Command<AvailabilityDto>(BeginEdit);
    public ICommand SaveEditCommand => new Command(async () => await SaveEditAsync());
    public ICommand CancelEditCommand => new Command(CancelEdit);
    public ICommand DeleteCommand => new Command<AvailabilityDto>(async slot => await DeleteAsync(slot));

    public async Task LoadAsync()
    {
        if (!_sessionState.IsEmployee)
        {
            HasError = true;
            StatusMessage = "Employee login is required.";
            return;
        }

        await RunAsync(async () =>
        {
            var slots = await _apiClient.GetMyAvailabilityAsync();
            Slots.Clear();

            foreach (var slot in slots.OrderBy(x => x.DayOfWeek).ThenBy(x => x.StartTime))
            {
                Slots.Add(slot);
            }
        });
    }

    private async Task AddAsync()
    {
        if (!TryParseTime(NewStartTime, out var start)
            || !TryParseTime(NewEndTime, out var end))
        {
            HasError = true;
            StatusMessage = "Invalid start or end time format (use HH:mm).";
            return;
        }

        await RunAsync(async () =>
        {
            await _apiClient.AddMyAvailabilityAsync(new UpsertAvailabilityRequest
            {
                DayOfWeek = NewDay.Value,
                StartTime = start,
                EndTime = end
            });

            await LoadAsync();
        }, "Availability added.");
    }

    private void BeginEdit(AvailabilityDto? slot)
    {
        if (slot is null)
        {
            return;
        }

        EditingAvailabilityId = slot.Id;
        EditDay = DayOptions.First(x => x.Value == slot.DayOfWeek);
        EditStartTime = slot.StartTime.ToString(@"hh\:mm");
        EditEndTime = slot.EndTime.ToString(@"hh\:mm");
    }

    private async Task SaveEditAsync()
    {
        if (!EditingAvailabilityId.HasValue)
        {
            HasError = true;
            StatusMessage = "Choose a slot to edit.";
            return;
        }

        if (!TryParseTime(EditStartTime, out var start)
            || !TryParseTime(EditEndTime, out var end))
        {
            HasError = true;
            StatusMessage = "Invalid start or end time format (use HH:mm).";
            return;
        }

        var availabilityId = EditingAvailabilityId.Value;

        await RunAsync(async () =>
        {
            await _apiClient.UpdateMyAvailabilityAsync(availabilityId, new UpsertAvailabilityRequest
            {
                DayOfWeek = EditDay.Value,
                StartTime = start,
                EndTime = end
            });

            CancelEdit();
            await LoadAsync();
        }, "Availability updated.");
    }

    private void CancelEdit()
    {
        EditingAvailabilityId = null;
    }

    private async Task DeleteAsync(AvailabilityDto? slot)
    {
        if (slot is null)
        {
            return;
        }

        await RunAsync(async () =>
        {
            await _apiClient.DeleteMyAvailabilityAsync(slot.Id);
            if (EditingAvailabilityId == slot.Id)
            {
                CancelEdit();
            }

            await LoadAsync();
        }, "Availability deleted.");
    }

    private static bool TryParseTime(string value, out TimeSpan result)
        => TimeSpan.TryParse(value, out result);

    private static List<DayOption> CreateDayOptions()
        => Enumerable.Range(0, 7)
            .Select(day => new DayOption
            {
                Value = day,
                Label = ((DayOfWeek)day).ToString()
            })
            .ToList();
}
