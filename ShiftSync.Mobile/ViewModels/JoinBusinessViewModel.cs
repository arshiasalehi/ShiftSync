using System.Windows.Input;
using ShiftSync.Mobile.Services;
using ShiftSync.Mobile.Views;
using ShiftSync.Shared.Contracts;

namespace ShiftSync.Mobile.ViewModels;

public sealed class JoinBusinessViewModel(ApiClient apiClient, SessionState sessionState) : BaseViewModel
{
    private string _companyCode = string.Empty;
    private string _fullName = string.Empty;
    private string _email = string.Empty;
    private string _password = string.Empty;

    public string CompanyCode
    {
        get => _companyCode;
        set => SetProperty(ref _companyCode, value);
    }

    public string FullName
    {
        get => _fullName;
        set => SetProperty(ref _fullName, value);
    }

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

    public ICommand RegisterEmployeeCommand => new Command(async () => await RegisterEmployeeAsync());
    public ICommand GoToLoginCommand => new Command(async () => await GoToLoginAsync());

    private async Task RegisterEmployeeAsync()
    {
        await RunAsync(async () =>
        {
            var response = await apiClient.RegisterEmployeeAsync(new RegisterEmployeeRequest
            {
                CompanyCode = CompanyCode,
                FullName = FullName,
                Email = Email,
                Password = Password
            });

            sessionState.SetCurrentUser(response);
            await Shell.Current.GoToAsync(nameof(ShiftsPage));
        }, "Employee account created.");
    }

    private static async Task GoToLoginAsync()
    {
        await Shell.Current.GoToAsync(nameof(LoginPage));
    }
}
