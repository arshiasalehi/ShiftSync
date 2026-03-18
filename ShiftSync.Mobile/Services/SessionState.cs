using ShiftSync.Shared.Contracts;

namespace ShiftSync.Mobile.Services;

public sealed class SessionState
{
    public AuthResponse? CurrentUser { get; private set; }
    public string? Token => CurrentUser?.Token;

    public bool IsAuthenticated => CurrentUser is not null;
    public bool IsEmployee => CurrentUser?.Role == UserRoles.Employee;
    public bool IsAdmin => CurrentUser?.Role == UserRoles.Admin;

    public void SetCurrentUser(AuthResponse response)
    {
        CurrentUser = response;
    }

    public void Logout()
    {
        CurrentUser = null;
    }
}
