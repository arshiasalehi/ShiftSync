using ShiftSync.Mobile.ViewModels;

namespace ShiftSync.Mobile.Views;

public partial class ShiftsPage : ContentPage
{
    private readonly ShiftsViewModel _viewModel;

    public ShiftsPage(ShiftsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadAsync();
    }
}
