using ShiftSync.Mobile.ViewModels;

namespace ShiftSync.Mobile.Views;

public partial class PayrollPage : ContentPage
{
    private readonly PayrollViewModel _viewModel;

    public PayrollPage(PayrollViewModel viewModel)
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
