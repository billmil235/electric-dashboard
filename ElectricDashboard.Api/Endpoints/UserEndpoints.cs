using System.Security.Claims;
using System.Threading.Tasks;
using ElectricDashboard.Services.User;
using ElectricDashboardApi.Dtos.User;
using ElectricDashboardApi.Models.User;
using ElectricDashboardApi.Shared.Extensions;
using Microsoft.AspNetCore.RateLimiting;

namespace ElectricDashboardApi.Endpoints;

public static class UserEndpoint
{
    public static RouteGroupBuilder RegisterUserEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("/register", async (UserDto user, IUserService userService, CancellationToken ct) =>
        {
            var result = await userService.CreateUserAsync(user, ct);

            if (result.EmailAlreadyExists)
            {
                return Results.Conflict(result.ErrorMessage);
            }

            return result.IsSuccessful ? Results.Ok() : Results.BadRequest(result.ErrorMessage);
        })
        .AllowAnonymous()
        .RequireRateLimiting("auth-limiter");

        group.MapGet("/email-exists/{email}", async (string email, IUserService userService, CancellationToken ct) =>
        {
            var exists = await userService.ExistsByEmailAsync(email, ct);
            return exists ? Results.Conflict(email) : Results.Ok();
        })
        .AllowAnonymous();

        group.MapPost("/login", async (Login login, IUserService userService, ILoginLockoutService lockoutService, HttpContext context, CancellationToken ct) =>
        {
            var ipAddress = context.Connection.RemoteIpAddress?.ToString();
            if (lockoutService.IsLockedOut(login.Username, ipAddress, out var retryAfter))
            {
                return Results.StatusCode(429); // Too Many Requests / Locked Out
            }

            var loginResult = await userService.LoginAsync(login.Username, login.Password, ct);

            if (loginResult.IsSuccessful)
            {
                lockoutService.ClearFailures(login.Username, ipAddress);
                return Results.Ok(loginResult.Token);
            }

            lockoutService.RegisterFailure(login.Username, ipAddress);
            return Results.BadRequest(loginResult.ErrorMessage);
        })
        .AllowAnonymous()
        .RequireRateLimiting("auth-limiter");

        group.MapPost("/refresh-token", async (RefreshTokenRequest request, IUserService userService, CancellationToken ct)
            => await userService.RefreshTokenAsync(request.RefreshToken, ct))
            .AllowAnonymous()
            .RequireRateLimiting("auth-limiter");

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
