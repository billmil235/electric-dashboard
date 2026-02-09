using ElectricDashboard.Models.Keycloak;
using UserModel = ElectricDashboard.Models.User.User;

namespace ElectricDashboard.Services.User;

public interface IUserService
{
    Task CreateUserAsync(UserModel userModel);

    Task<LoginTokenResponse> LoginAsync(string username, string password);

    Task<RefreshTokenResponse?> RefreshTokenAsync(string refreshToken);

    Task UpdateUserProfile(UserModel user, Guid userId);
}
