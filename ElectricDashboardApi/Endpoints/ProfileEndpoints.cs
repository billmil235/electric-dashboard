namespace ElectricDashboardApi.Endpoints;

public static class ProfileEndpoints
{
    public static RouteGroupBuilder RegisterProfileEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/address", () => Results.Ok()).RequireAuthorization();
        
        group.MapPost("/address", () => Results.Ok()).RequireAuthorization();

        group.MapDelete("/address", () => Results.Ok()).RequireAuthorization();
        
        return group;
    }
}