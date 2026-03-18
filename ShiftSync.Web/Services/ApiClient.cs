using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using ShiftSync.Shared.Contracts;

namespace ShiftSync.Web.Services;

public sealed class ApiClient(HttpClient httpClient, SessionState sessionState)
{
    public Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
        => SendAsync<AuthResponse>(HttpMethod.Post, "api/auth/login", request, false, cancellationToken);

    public Task<AuthResponse> RegisterAdminAsync(RegisterAdminRequest request, CancellationToken cancellationToken = default)
        => SendAsync<AuthResponse>(HttpMethod.Post, "api/auth/register-admin", request, false, cancellationToken);

    public Task<AuthResponse> RegisterEmployeeAsync(RegisterEmployeeRequest request, CancellationToken cancellationToken = default)
        => SendAsync<AuthResponse>(HttpMethod.Post, "api/auth/register-employee", request, false, cancellationToken);

    public Task<List<EmployeeDto>> GetEmployeesAsync(CancellationToken cancellationToken = default)
        => SendAsync<List<EmployeeDto>>(HttpMethod.Get, "api/employees", null, true, cancellationToken);

    public Task<List<RoleTypeDto>> GetRoleTypesAsync(CancellationToken cancellationToken = default)
        => SendAsync<List<RoleTypeDto>>(HttpMethod.Get, "api/employees/role-types", null, true, cancellationToken);

    public Task<EmployeeDto> UpdatePayRateAsync(int employeeId, UpdatePayRateRequest request, CancellationToken cancellationToken = default)
        => SendAsync<EmployeeDto>(HttpMethod.Put, $"api/employees/{employeeId}/payrate", request, true, cancellationToken);

    public Task<EmployeeDto> UpdateRolesAsync(int employeeId, UpdateEmployeeRolesRequest request, CancellationToken cancellationToken = default)
        => SendAsync<EmployeeDto>(HttpMethod.Put, $"api/employees/{employeeId}/roles", request, true, cancellationToken);

    public Task<List<AvailabilityDto>> GetAdminAvailabilityAsync(CancellationToken cancellationToken = default)
        => SendAsync<List<AvailabilityDto>>(HttpMethod.Get, "api/availability", null, true, cancellationToken);

    public Task<WeeklyScheduleDto> CreateScheduleAsync(CreateScheduleRequest request, CancellationToken cancellationToken = default)
        => SendAsync<WeeklyScheduleDto>(HttpMethod.Post, "api/schedules", request, true, cancellationToken);

    public Task<WeeklyScheduleDto> GetScheduleAsync(DateTime weekStartDate, CancellationToken cancellationToken = default)
        => SendAsync<WeeklyScheduleDto>(HttpMethod.Get, $"api/schedules/{weekStartDate:yyyy-MM-dd}", null, true, cancellationToken);

    public Task<ShiftDto> CreateShiftAsync(CreateShiftRequest request, CancellationToken cancellationToken = default)
        => SendAsync<ShiftDto>(HttpMethod.Post, "api/shifts", request, true, cancellationToken);

    public Task<List<ShiftDto>> GetMyShiftsAsync(DateTime? weekStartDate = null, CancellationToken cancellationToken = default)
    {
        var path = weekStartDate.HasValue
            ? $"api/me/shifts?weekStart={weekStartDate:yyyy-MM-dd}"
            : "api/me/shifts";

        return SendAsync<List<ShiftDto>>(HttpMethod.Get, path, null, true, cancellationToken);
    }

    public Task<PayrollEstimateDto> GetMyPayrollAsync(DateTime? weekStartDate = null, CancellationToken cancellationToken = default)
    {
        var path = weekStartDate.HasValue
            ? $"api/me/payroll?weekStart={weekStartDate:yyyy-MM-dd}"
            : "api/me/payroll";

        return SendAsync<PayrollEstimateDto>(HttpMethod.Get, path, null, true, cancellationToken);
    }

    public Task<List<AvailabilityDto>> GetMyAvailabilityAsync(CancellationToken cancellationToken = default)
        => SendAsync<List<AvailabilityDto>>(HttpMethod.Get, "api/me/availability", null, true, cancellationToken);

    public Task<AvailabilityDto> AddMyAvailabilityAsync(UpsertAvailabilityRequest request, CancellationToken cancellationToken = default)
        => SendAsync<AvailabilityDto>(HttpMethod.Post, "api/me/availability", request, true, cancellationToken);

    public Task<AvailabilityDto> UpdateMyAvailabilityAsync(int availabilityId, UpsertAvailabilityRequest request, CancellationToken cancellationToken = default)
        => SendAsync<AvailabilityDto>(HttpMethod.Put, $"api/me/availability/{availabilityId}", request, true, cancellationToken);

    public Task DeleteMyAvailabilityAsync(int availabilityId, CancellationToken cancellationToken = default)
        => SendAsync(HttpMethod.Delete, $"api/me/availability/{availabilityId}", null, true, cancellationToken);

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
            return payload ?? throw new InvalidOperationException("The server returned an empty response.");
        }

        throw await CreateExceptionAsync(response, cancellationToken);
    }

    private async Task SendAsync(
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
            message = response.StatusCode == HttpStatusCode.Unauthorized
                ? "Unauthorized request."
                : $"Request failed with status {(int)response.StatusCode}.";
        }

        return new InvalidOperationException(message);
    }

    private void EnsureAuthenticated()
    {
        if (string.IsNullOrWhiteSpace(sessionState.Token))
        {
            throw new InvalidOperationException("You need to login first.");
        }
    }
}
