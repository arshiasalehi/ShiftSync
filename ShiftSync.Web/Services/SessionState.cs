using ShiftSync.Shared.Contracts;

namespace ShiftSync.Web.Services;

public sealed class SessionState
{
    public AuthResponse? CurrentUser { get; private set; }
    public string? Token => CurrentUser?.Token;

    public bool IsAuthenticated => CurrentUser is not null;
    public bool IsAdmin => CurrentUser?.Role == UserRoles.Admin;
    public bool IsEmployee => CurrentUser?.Role == UserRoles.Employee;

    public event Action? Changed;

    public void SetUser(AuthResponse response)
    {
        CurrentUser = response;
        Changed?.Invoke();
    }

    public void Logout()
    {
        CurrentUser = null;
        Changed?.Invoke();
    }
}
