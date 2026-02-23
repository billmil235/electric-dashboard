using ElectricDashboardApi.Dtos.OAuth;
using ElectricDashboardApi.Dtos.User;

namespace ElectricDashboard.Services.User;

public interface IUserService
{
    Task<CreateUserResult> CreateUserAsync(UserDto userModel);

    Task<LoginResult> LoginAsync(string username, string password);

    Task<RefreshTokenResponse?> RefreshTokenAsync(string refreshToken);

    Task UpdateUserProfile(UserDto user, Guid userId);

    Task<UserDto> GetUserInformation(Guid userGuid);
}
