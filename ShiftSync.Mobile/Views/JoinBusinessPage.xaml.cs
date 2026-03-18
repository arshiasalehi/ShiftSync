using ShiftSync.Mobile.ViewModels;

namespace ShiftSync.Mobile.Views;

public partial class JoinBusinessPage : ContentPage
{
    public JoinBusinessPage(JoinBusinessViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
