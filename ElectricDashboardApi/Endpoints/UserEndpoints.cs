namespace ElectricDashboardApi.Endpoints;

public static class UserEndpoint
{
    public static RouteGroupBuilder RegisterUserEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("/register", () => Results.Ok()).AllowAnonymous();

        group.MapPost("/login", () => Results.Ok()).AllowAnonymous();

        return group;
    }
}