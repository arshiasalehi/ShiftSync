using Microsoft.EntityFrameworkCore;
using ShiftSync.Api.Data;
using ShiftSync.Api.Models;
using ShiftSync.Api.Repositories.Interfaces;
using ShiftSync.Shared.Contracts;

namespace ShiftSync.Api.Repositories;

public sealed class UserRepository(ShiftSyncDbContext dbContext) : IUserRepository
{
    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        => dbContext.Users
            .Include(x => x.Business)
            .Include(x => x.EmployeeProfile)
            .Include(x => x.EmployeeRoles)
            .ThenInclude(x => x.RoleType)
            .FirstOrDefaultAsync(x => x.Email == email, cancellationToken);

    public Task<User?> GetByIdAsync(int userId, CancellationToken cancellationToken = default)
        => dbContext.Users
            .Include(x => x.Business)
            .Include(x => x.EmployeeProfile)
            .Include(x => x.EmployeeRoles)
            .ThenInclude(x => x.RoleType)
            .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);

    public Task<User?> GetEmployeeByIdAsync(int businessId, int employeeId, CancellationToken cancellationToken = default)
        => dbContext.Users
            .Include(x => x.EmployeeProfile)
            .Include(x => x.EmployeeRoles)
            .ThenInclude(x => x.RoleType)
            .FirstOrDefaultAsync(
                x => x.BusinessId == businessId
                     && x.Id == employeeId
                     && x.Role == UserRoles.Employee,
                cancellationToken);

    public Task<List<User>> GetEmployeesByBusinessAsync(int businessId, CancellationToken cancellationToken = default)
        => dbContext.Users
            .Include(x => x.EmployeeProfile)
            .Include(x => x.EmployeeRoles)
            .ThenInclude(x => x.RoleType)
            .Where(x => x.BusinessId == businessId && x.Role == UserRoles.Employee)
            .OrderBy(x => x.FullName)
            .ToListAsync(cancellationToken);

    public Task AddAsync(User user, CancellationToken cancellationToken = default)
        => dbContext.Users.AddAsync(user, cancellationToken).AsTask();

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => dbContext.SaveChangesAsync(cancellationToken);
}
