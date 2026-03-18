using System.Windows.Input;
using ShiftSync.Mobile.Services;
using ShiftSync.Mobile.Views;
using ShiftSync.Shared.Contracts;

namespace ShiftSync.Mobile.ViewModels;

public sealed class LoginViewModel(ApiClient apiClient, SessionState sessionState) : BaseViewModel
{
    private string _email = string.Empty;
    private string _password = string.Empty;

    private string _adminBusinessName = string.Empty;
    private string _adminFullName = string.Empty;
    private string _adminEmail = string.Empty;
    private string _adminPassword = string.Empty;

    public string Email
    {
        get => _email;
        set => SetProperty(ref _email, value);
    }

    public string Password
    {
        get => _password;
        set => SetProperty(ref _password, value);
    }

    public string AdminBusinessName
    {
        get => _adminBusinessName;
        set => SetProperty(ref _adminBusinessName, value);
    }

    public string AdminFullName
    {
        get => _adminFullName;
        set => SetProperty(ref _adminFullName, value);
    }

    public string AdminEmail
    {
        get => _adminEmail;
        set => SetProperty(ref _adminEmail, value);
    }

    public string AdminPassword
    {
        get => _adminPassword;
        set => SetProperty(ref _adminPassword, value);
    }

    public ICommand LoginCommand => new Command(async () => await LoginAsync());
    public ICommand RegisterAdminCommand => new Command(async () => await RegisterAdminAsync());
    public ICommand GoToJoinBusinessCommand => new Command(async () => await GoToJoinBusinessAsync());

    private async Task LoginAsync()
    {
        await RunAsync(async () =>
        {
            var response = await apiClient.LoginAsync(new LoginRequest
            {
                Email = Email,
                Password = Password
            });

            sessionState.SetCurrentUser(response);

            if (response.Role == UserRoles.Employee)
            {
                await Shell.Current.GoToAsync(nameof(ShiftsPage));
            }
            else
            {
                StatusMessage = "Admin account signed in. Admin tools are available in the web app.";
            }
        });
    }

    private async Task RegisterAdminAsync()
    {
        await RunAsync(async () =>
        {
            var response = await apiClient.RegisterAdminAsync(new RegisterAdminRequest
            {
                BusinessName = AdminBusinessName,
                FullName = AdminFullName,
                Email = AdminEmail,
                Password = AdminPassword
            });

            sessionState.SetCurrentUser(response);
            StatusMessage = $"Admin account created. Company code: {response.CompanyCode}";
        });
    }

    private static async Task GoToJoinBusinessAsync()
    {
        await Shell.Current.GoToAsync(nameof(JoinBusinessPage));
    }
}
