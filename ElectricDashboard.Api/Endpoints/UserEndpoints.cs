using System.Security.Claims;
using ElectricDashboard.Services.User;
using ElectricDashboardApi.Dtos.User;
using ElectricDashboardApi.Models.User;
using ElectricDashboardApi.Shared.Extensions;

namespace ElectricDashboardApi.Endpoints;

public static class UserEndpoint
{
    public static RouteGroupBuilder RegisterUserEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("/register", async (UserDto user, IUserService userService) =>
        {
            var result = await userService.CreateUserAsync(user);

            return result.IsSuccessful ? Results.Ok() : Results.BadRequest(result.ErrorMessage);
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
                var userModel = new UserDto()
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

        group.MapGet("/profile", (ClaimsPrincipal userClaims) => Results.Ok());

        return group;
    }
}
