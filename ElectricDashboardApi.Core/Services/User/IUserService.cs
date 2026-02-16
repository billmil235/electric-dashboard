using ElectricDashboardApi.Dtos.OAuth;
using ElectricDashboardApi.Dtos.User;

namespace ElectricDashboard.Services.User;

public interface IUserService
{
    Task CreateUserAsync(UserDto userModel);

    Task<LoginTokenResponse> LoginAsync(string username, string password);

    Task<RefreshTokenResponse?> RefreshTokenAsync(string refreshToken);

    Task UpdateUserProfile(UserDto user, Guid userId);
}
