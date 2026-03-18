using System.Net.Http.Headers;
using System.Net.Http.Json;
using ShiftSync.Shared.Contracts;

namespace ShiftSync.Mobile.Services;

public sealed class ApiClient(HttpClient httpClient, SessionState sessionState)
{
    public Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
        => SendAsync<AuthResponse>(HttpMethod.Post, "api/auth/login", request, false, cancellationToken);

    public Task<AuthResponse> RegisterAdminAsync(RegisterAdminRequest request, CancellationToken cancellationToken = default)
        => SendAsync<AuthResponse>(HttpMethod.Post, "api/auth/register-admin", request, false, cancellationToken);

    public Task<AuthResponse> RegisterEmployeeAsync(RegisterEmployeeRequest request, CancellationToken cancellationToken = default)
        => SendAsync<AuthResponse>(HttpMethod.Post, "api/auth/register-employee", request, false, cancellationToken);

    public Task<List<AvailabilityDto>> GetMyAvailabilityAsync(CancellationToken cancellationToken = default)
        => SendAsync<List<AvailabilityDto>>(HttpMethod.Get, "api/me/availability", null, true, cancellationToken);

    public Task<AvailabilityDto> AddMyAvailabilityAsync(UpsertAvailabilityRequest request, CancellationToken cancellationToken = default)
        => SendAsync<AvailabilityDto>(HttpMethod.Post, "api/me/availability", request, true, cancellationToken);

    public Task<AvailabilityDto> UpdateMyAvailabilityAsync(int availabilityId, UpsertAvailabilityRequest request, CancellationToken cancellationToken = default)
        => SendAsync<AvailabilityDto>(HttpMethod.Put, $"api/me/availability/{availabilityId}", request, true, cancellationToken);

    public Task DeleteMyAvailabilityAsync(int availabilityId, CancellationToken cancellationToken = default)
        => SendNoContentAsync(HttpMethod.Delete, $"api/me/availability/{availabilityId}", true, cancellationToken);

    public Task<List<ShiftDto>> GetMyShiftsAsync(DateTime weekStartDate, CancellationToken cancellationToken = default)
        => SendAsync<List<ShiftDto>>(HttpMethod.Get, $"api/me/shifts?weekStart={weekStartDate:yyyy-MM-dd}", null, true, cancellationToken);

    public Task<PayrollEstimateDto> GetMyPayrollAsync(DateTime weekStartDate, CancellationToken cancellationToken = default)
        => SendAsync<PayrollEstimateDto>(HttpMethod.Get, $"api/me/payroll?weekStart={weekStartDate:yyyy-MM-dd}", null, true, cancellationToken);

    private async Task<T> SendAsync<T>(
        HttpMethod method,
        string path,
        object? body,
        bool requiresAuth,
        CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(method, path);

        if (body is not null)
        {
            request.Content = JsonContent.Create(body);
        }

        if (requiresAuth)
        {
            EnsureAuthenticated();
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", sessionState.Token);
        }

        using var response = await httpClient.SendAsync(request, cancellationToken);
        if (response.IsSuccessStatusCode)
        {
            var payload = await response.Content.ReadFromJsonAsync<T>(cancellationToken: cancellationToken);
            return payload ?? throw new InvalidOperationException("Server returned empty payload.");
        }

        throw await CreateExceptionAsync(response, cancellationToken);
    }

    private async Task SendNoContentAsync(
        HttpMethod method,
        string path,
        bool requiresAuth,
        CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(method, path);

        if (requiresAuth)
        {
            EnsureAuthenticated();
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", sessionState.Token);
        }

        using var response = await httpClient.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            throw await CreateExceptionAsync(response, cancellationToken);
        }
    }

    private static async Task<Exception> CreateExceptionAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        var payload = await response.Content.ReadFromJsonAsync<ApiError>(cancellationToken: cancellationToken);
        var message = payload?.Message;
        if (string.IsNullOrWhiteSpace(message))
        {
            message = $"Request failed ({(int)response.StatusCode}).";
        }

        return new InvalidOperationException(message);
    }

    private void EnsureAuthenticated()
    {
        if (string.IsNullOrWhiteSpace(sessionState.Token))
        {
            throw new InvalidOperationException("You must login first.");
        }
    }
}
