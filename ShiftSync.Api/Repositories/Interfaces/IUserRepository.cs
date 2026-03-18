using ShiftSync.Api.Models;

namespace ShiftSync.Api.Repositories.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<User?> GetByIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<User?> GetEmployeeByIdAsync(int businessId, int employeeId, CancellationToken cancellationToken = default);
    Task<List<User>> GetEmployeesByBusinessAsync(int businessId, CancellationToken cancellationToken = default);
    Task AddAsync(User user, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
