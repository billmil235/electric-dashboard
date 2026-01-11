using ElectricDashboard.Models.User;
using ElectricDashboard.Services.User;

namespace ElectricDashboardApi.Endpoints;

public static class UserEndpoint
{
    public static RouteGroupBuilder RegisterUserEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("/register", async (User user, IUserService userService) =>
        {
            await userService.CreateUserAsync(user);
            return Results.Ok();
        }).AllowAnonymous();

        group.MapPost("/login", async (Login login, IUserService userService) =>
        {
            var token = await userService.LoginAsync(login.Username, login.Password);
            return Results.Ok(token);
        }).AllowAnonymous();

        group.MapPost("/refresh-token/{token:string}", async (string token, IUserService userService) 
            => await userService.RefreshTokenAsync(token));
        
        return group;
    }
}