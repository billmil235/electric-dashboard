using System.Security.Claims;
using ElectricDashboard.Models.User;
using ElectricDashboard.Services.User;
using ElectricDashboardApi.Shared.Extensions;

namespace ElectricDashboardApi.Endpoints;

public static class UserEndpoint
{
    public static RouteGroupBuilder RegisterUserEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("/register", async (User user, IUserService userService) =>
        {
            await userService.CreateUserAsync(user);
            return Results.Ok();
        })
            .AllowAnonymous();

        group.MapPost("/login", async (Login login, IUserService userService) =>
        {
            var token = await userService.LoginAsync(login.Username, login.Password);
            return Results.Ok(token);
        })
            .AllowAnonymous();
        
        group.MapPost("/refresh-token/{token}", async (string token, IUserService userService) 
            => await userService.RefreshTokenAsync(token))
            .AllowAnonymous();

        group.MapPost("/update-profile", (UserUpdate user, ClaimsPrincipal userClaims, IUserService userService) =>
            {
                var userId = userClaims.GetGuid();
                var userModel = new User()
                {
                    EmailAddress = string.Empty,
                    Password = string.Empty,
                    DateOfBirth = user.DateOfBirth,
                    FirstName = user.FirstName,
                    LastName = user.LastName
                };
                return userService.UpdateUserProfile(userModel, userId);
            })
            .RequireAuthorization();
        
        return group;
    }
}