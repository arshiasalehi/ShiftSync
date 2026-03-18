using ShiftSync.Api.Infrastructure;
using ShiftSync.Api.Models;
using ShiftSync.Api.Repositories.Interfaces;
using ShiftSync.Api.Services.Interfaces;
using ShiftSync.Shared.Contracts;

namespace ShiftSync.Api.Services;

public sealed class AuthService(
    IBusinessRepository businessRepository,
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IJwtTokenService jwtTokenService) : IAuthService
{
    public async Task<AuthResponse> RegisterAdminAsync(RegisterAdminRequest request, CancellationToken cancellationToken = default)
    {
        ValidateRegisterInput(request.FullName, request.Email, request.Password);
        if (string.IsNullOrWhiteSpace(request.BusinessName))
        {
            throw new ApiException("Business name is required.");
        }

        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        var existingUser = await userRepository.GetByEmailAsync(normalizedEmail, cancellationToken);
        if (existingUser is not null)
        {
            throw new ApiException("An account with this email already exists.", StatusCodes.Status409Conflict);
        }

        var companyCode = await GenerateCompanyCodeAsync(cancellationToken);
        var business = new Business
        {
            Name = request.BusinessName.Trim(),
            CompanyCode = companyCode
        };

        var adminUser = new User
        {
            FullName = request.FullName.Trim(),
            Email = normalizedEmail,
            PasswordHash = passwordHasher.Hash(request.Password),
            Role = UserRoles.Admin,
            Business = business
        };

        business.Users.Add(adminUser);

        await businessRepository.AddAsync(business, cancellationToken);
        await businessRepository.SaveChangesAsync(cancellationToken);

        return CreateAuthResponse(adminUser, business.CompanyCode);
    }

    public async Task<AuthResponse> RegisterEmployeeAsync(RegisterEmployeeRequest request, CancellationToken cancellationToken = default)
    {
        ValidateRegisterInput(request.FullName, request.Email, request.Password);

        if (string.IsNullOrWhiteSpace(request.CompanyCode))
        {
            throw new ApiException("Company code is required.");
        }

        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var existingUser = await userRepository.GetByEmailAsync(normalizedEmail, cancellationToken);
        if (existingUser is not null)
        {
            throw new ApiException("An account with this email already exists.", StatusCodes.Status409Conflict);
        }

        var business = await businessRepository.GetByCompanyCodeAsync(request.CompanyCode.Trim().ToUpperInvariant(), cancellationToken);
        if (business is null)
        {
            throw new ApiException("Company code is invalid.", StatusCodes.Status404NotFound);
        }

        var employee = new User
        {
            FullName = request.FullName.Trim(),
            Email = normalizedEmail,
            PasswordHash = passwordHasher.Hash(request.Password),
            Role = UserRoles.Employee,
            BusinessId = business.Id,
            EmployeeProfile = new EmployeeProfile
            {
                HourlyRate = 0,
                MaxWeeklyHours = 40
            }
        };

        await userRepository.AddAsync(employee, cancellationToken);
        await userRepository.SaveChangesAsync(cancellationToken);

        employee.Business = business;
        return CreateAuthResponse(employee, business.CompanyCode);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            throw new ApiException("Email and password are required.");
        }

        var user = await userRepository.GetByEmailAsync(request.Email.Trim().ToLowerInvariant(), cancellationToken);
        if (user is null || !passwordHasher.Verify(user.PasswordHash, request.Password))
        {
            throw new ApiException("Invalid email or password.", StatusCodes.Status401Unauthorized);
        }

        return CreateAuthResponse(user, user.Business.CompanyCode);
    }

    private AuthResponse CreateAuthResponse(User user, string companyCode)
        => new()
        {
            Token = jwtTokenService.CreateToken(user, companyCode),
            UserId = user.Id,
            BusinessId = user.BusinessId,
            CompanyCode = companyCode,
            FullName = user.FullName,
            Email = user.Email,
            Role = user.Role
        };

    private static void ValidateRegisterInput(string fullName, string email, string password)
    {
        if (string.IsNullOrWhiteSpace(fullName))
        {
            throw new ApiException("Full name is required.");
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ApiException("Email is required.");
        }

        if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
        {
            throw new ApiException("Password must be at least 6 characters.");
        }
    }

    private async Task<string> GenerateCompanyCodeAsync(CancellationToken cancellationToken)
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";

        for (var attempt = 0; attempt < 20; attempt++)
        {
            var code = new string(Enumerable.Range(0, 6)
                .Select(_ => chars[Random.Shared.Next(chars.Length)])
                .ToArray());

            if (!await businessRepository.CompanyCodeExistsAsync(code, cancellationToken))
            {
                return code;
            }
        }

        throw new ApiException("Could not generate a unique company code.", StatusCodes.Status500InternalServerError);
    }
}
