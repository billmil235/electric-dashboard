namespace ElectricDashboardApi.Endpoints;

public static class DataEndpoints
{
    public static RouteGroupBuilder RegisterDataEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/electric-bill", () => Results.Ok()).RequireAuthorization();

        group.MapPost("/electric-bill", () => Results.Ok()).RequireAuthorization();

        return group;
    }
}