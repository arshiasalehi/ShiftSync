using System.Security.Claims;

namespace ShiftSync.Api.Infrastructure;

public static class ClaimsPrincipalExtensions
{
    public static int GetUserId(this ClaimsPrincipal user)
    {
        var raw = user.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new ApiException("Invalid token payload.", StatusCodes.Status401Unauthorized);

        return int.TryParse(raw, out var id)
            ? id
            : throw new ApiException("Invalid token payload.", StatusCodes.Status401Unauthorized);
    }

    public static int GetBusinessId(this ClaimsPrincipal user)
    {
        var raw = user.FindFirstValue("businessId")
            ?? throw new ApiException("Invalid token payload.", StatusCodes.Status401Unauthorized);

        return int.TryParse(raw, out var id)
            ? id
            : throw new ApiException("Invalid token payload.", StatusCodes.Status401Unauthorized);
    }
}
