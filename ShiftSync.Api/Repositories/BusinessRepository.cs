using Microsoft.EntityFrameworkCore;
using ShiftSync.Api.Data;
using ShiftSync.Api.Models;
using ShiftSync.Api.Repositories.Interfaces;

namespace ShiftSync.Api.Repositories;

public sealed class BusinessRepository(ShiftSyncDbContext dbContext) : IBusinessRepository
{
    public Task<Business?> GetByCompanyCodeAsync(string companyCode, CancellationToken cancellationToken = default)
        => dbContext.Businesses.FirstOrDefaultAsync(x => x.CompanyCode == companyCode, cancellationToken);

    public Task<Business?> GetByIdAsync(int businessId, CancellationToken cancellationToken = default)
        => dbContext.Businesses.FirstOrDefaultAsync(x => x.Id == businessId, cancellationToken);

    public Task AddAsync(Business business, CancellationToken cancellationToken = default)
        => dbContext.Businesses.AddAsync(business, cancellationToken).AsTask();

    public Task<bool> CompanyCodeExistsAsync(string companyCode, CancellationToken cancellationToken = default)
        => dbContext.Businesses.AnyAsync(x => x.CompanyCode == companyCode, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => dbContext.SaveChangesAsync(cancellationToken);
}
