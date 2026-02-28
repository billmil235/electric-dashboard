using System.Security.Claims;
using ElectricDashboardApi.Infrastructure.Services.Forecast;
using ElectricDashboardApi.Shared.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace ElectricDashboardApi.Endpoints;

public static class ForecastEndpoints
{
    public static RouteGroupBuilder RegisterForecastEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/{addressGuid:guid}", async ([FromRoute] Guid addressGuid, ClaimsPrincipal user, IForecastService forecastService) =>
        {
            var userId = user.GetGuid();
            try
            {
                var response = await forecastService.GetForecastAsync(addressGuid, userId);
                return Results.Ok(response);
            }
            catch(UnauthorizedAccessException)
            {
                return Results.Forbid();
            }
            catch(Exception ex)
            {
                return Results.Problem(ex.Message);
            }
        })
        .RequireAuthorization();
        return group;
    }
}
