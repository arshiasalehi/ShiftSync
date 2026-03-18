using ShiftSync.Mobile.Views;

namespace ShiftSync.Mobile;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
        Routing.RegisterRoute(nameof(JoinBusinessPage), typeof(JoinBusinessPage));
        Routing.RegisterRoute(nameof(AvailabilityPage), typeof(AvailabilityPage));
        Routing.RegisterRoute(nameof(ShiftsPage), typeof(ShiftsPage));
        Routing.RegisterRoute(nameof(PayrollPage), typeof(PayrollPage));
    }
}
