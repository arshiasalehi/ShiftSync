using ShiftSync.Api.Models;

namespace ShiftSync.Api.Repositories.Interfaces;

public interface IAvailabilityRepository
{
    Task<List<Availability>> GetForEmployeeAsync(int employeeId, CancellationToken cancellationToken = default);
    Task<List<Availability>> GetForBusinessAsync(int businessId, CancellationToken cancellationToken = default);
    Task<Availability?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task AddAsync(Availability availability, CancellationToken cancellationToken = default);
    void Remove(Availability availability);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
