using ShiftSync.Api.Models;

namespace ShiftSync.Api.Infrastructure;

public interface IJwtTokenService
{
    string CreateToken(User user, string companyCode);
}
