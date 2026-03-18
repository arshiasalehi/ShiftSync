using ShiftSync.Shared.Contracts;

namespace ShiftSync.Api.Services.Interfaces;

public interface IAuthService
{
    Task<AuthResponse> RegisterAdminAsync(RegisterAdminRequest request, CancellationToken cancellationToken = default);
    Task<AuthResponse> RegisterEmployeeAsync(RegisterEmployeeRequest request, CancellationToken cancellationToken = default);
    Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
}
