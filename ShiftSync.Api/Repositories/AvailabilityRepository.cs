using Microsoft.EntityFrameworkCore;
using ShiftSync.Api.Data;
using ShiftSync.Api.Models;
using ShiftSync.Api.Repositories.Interfaces;

namespace ShiftSync.Api.Repositories;

public sealed class AvailabilityRepository(ShiftSyncDbContext dbContext) : IAvailabilityRepository
{
    public Task<List<Availability>> GetForEmployeeAsync(int employeeId, CancellationToken cancellationToken = default)
        => dbContext.Availabilities
            .Where(x => x.EmployeeId == employeeId)
            .OrderBy(x => x.DayOfWeek)
            .ThenBy(x => x.StartTime)
            .ToListAsync(cancellationToken);

    public Task<List<Availability>> GetForBusinessAsync(int businessId, CancellationToken cancellationToken = default)
        => dbContext.Availabilities
            .Include(x => x.Employee)
            .Where(x => x.Employee.BusinessId == businessId)
            .OrderBy(x => x.Employee.FullName)
            .ThenBy(x => x.DayOfWeek)
            .ThenBy(x => x.StartTime)
            .ToListAsync(cancellationToken);

    public Task<Availability?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => dbContext.Availabilities
            .Include(x => x.Employee)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task AddAsync(Availability availability, CancellationToken cancellationToken = default)
        => dbContext.Availabilities.AddAsync(availability, cancellationToken).AsTask();

    public void Remove(Availability availability)
        => dbContext.Availabilities.Remove(availability);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => dbContext.SaveChangesAsync(cancellationToken);
}
