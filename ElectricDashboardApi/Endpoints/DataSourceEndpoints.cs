namespace ElectricDashboardApi.Endpoints;

public static class DataSourceEndpoints
{
    public static RouteGroupBuilder RegisterDataEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/electric-bill", () => Results.Ok()).RequireAuthorization();

        group.MapPost("/electric-bill", () => Results.Ok()).RequireAuthorization();

        return group;
    }
}