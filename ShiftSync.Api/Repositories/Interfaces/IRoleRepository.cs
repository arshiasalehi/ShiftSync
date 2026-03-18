using ShiftSync.Api.Models;

namespace ShiftSync.Api.Repositories.Interfaces;

public interface IRoleRepository
{
    Task<List<RoleType>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<RoleType?> GetByIdAsync(int roleTypeId, CancellationToken cancellationToken = default);
    Task<List<RoleType>> GetByIdsAsync(IEnumerable<int> roleTypeIds, CancellationToken cancellationToken = default);
}
