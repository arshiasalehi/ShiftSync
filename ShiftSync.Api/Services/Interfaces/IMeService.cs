using ShiftSync.Shared.Contracts;

namespace ShiftSync.Api.Services.Interfaces;

public interface IMeService
{
    Task<List<ShiftDto>> GetShiftsAsync(int userId, DateTime? weekStartDate = null, CancellationToken cancellationToken = default);
    Task<PayrollEstimateDto> GetPayrollAsync(int userId, DateTime? weekStartDate = null, CancellationToken cancellationToken = default);
    Task<List<AvailabilityDto>> GetAvailabilityAsync(int userId, CancellationToken cancellationToken = default);
    Task<AvailabilityDto> AddAvailabilityAsync(int userId, UpsertAvailabilityRequest request, CancellationToken cancellationToken = default);
    Task<AvailabilityDto> UpdateAvailabilityAsync(int userId, int availabilityId, UpsertAvailabilityRequest request, CancellationToken cancellationToken = default);
    Task DeleteAvailabilityAsync(int userId, int availabilityId, CancellationToken cancellationToken = default);
}
