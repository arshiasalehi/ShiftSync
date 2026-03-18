using ShiftSync.Api.Models;

namespace ShiftSync.Api.Repositories.Interfaces;

public interface IBusinessRepository
{
    Task<Business?> GetByCompanyCodeAsync(string companyCode, CancellationToken cancellationToken = default);
    Task<Business?> GetByIdAsync(int businessId, CancellationToken cancellationToken = default);
    Task AddAsync(Business business, CancellationToken cancellationToken = default);
    Task<bool> CompanyCodeExistsAsync(string companyCode, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
