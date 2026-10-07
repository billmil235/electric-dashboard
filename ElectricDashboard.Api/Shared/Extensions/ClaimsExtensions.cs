using System.Security.Claims;

namespace ElectricDashboardApi.Shared.Extensions;

public static class ClaimsExtensions
{
    public static Guid GetGuid(this ClaimsPrincipal user)
    {
        var claim = user.FindFirst(ClaimTypes.NameIdentifier);
        if (claim == null || string.IsNullOrEmpty(claim.Value))
        {
            throw new UnauthorizedAccessException("User identity not found in claims.");
        }
        return new Guid(claim.Value);
    }
}