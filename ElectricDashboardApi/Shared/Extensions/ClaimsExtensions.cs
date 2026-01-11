using System.Security.Claims;

namespace ElectricDashboardApi.Shared.Extensions;

public static class ClaimsExtensions
{
    public static Guid GetGuid(this ClaimsPrincipal user)
    {
        var guid = user.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
        var userId = new Guid(guid);
        return userId;
    }
}