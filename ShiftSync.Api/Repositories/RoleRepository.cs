using Microsoft.EntityFrameworkCore;
using ShiftSync.Api.Data;
using ShiftSync.Api.Models;
using ShiftSync.Api.Repositories.Interfaces;

namespace ShiftSync.Api.Repositories;

public sealed class RoleRepository(ShiftSyncDbContext dbContext) : IRoleRepository
{
    public Task<List<RoleType>> GetAllAsync(CancellationToken cancellationToken = default)
        => dbContext.RoleTypes.OrderBy(x => x.Name).ToListAsync(cancellationToken);

    public Task<RoleType?> GetByIdAsync(int roleTypeId, CancellationToken cancellationToken = default)
        => dbContext.RoleTypes.FirstOrDefaultAsync(x => x.Id == roleTypeId, cancellationToken);

    public Task<List<RoleType>> GetByIdsAsync(IEnumerable<int> roleTypeIds, CancellationToken cancellationToken = default)
        => dbContext.RoleTypes.Where(x => roleTypeIds.Contains(x.Id)).ToListAsync(cancellationToken);
}
