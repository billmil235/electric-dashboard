using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace ElectricDashboardApi.Endpoints;

public static class ProfileEndpoints
{
    public static RouteGroupBuilder RegisterProfileEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/address", (ClaimsPrincipal user) => Results.Ok()).RequireAuthorization();
        
        group.MapPost("/address", (ClaimsPrincipal user) => Results.Ok()).RequireAuthorization();

        group.MapDelete("/address/{addressId:Guid}", ([FromRoute] Guid addressId, ClaimsPrincipal user) => Results.Ok()).RequireAuthorization();
        
        return group;
    }
}