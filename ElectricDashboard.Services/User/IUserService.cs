using ElectricDashboard.Models.Keycloak;
using UserModel = ElectricDashboard.Models.User.User;

namespace ElectricDashboard.Services.User;

public interface IUserService
{
    Task CreateUserAsync(Models.User.User userModel);
    
    Task<string> LoginAsync(string username, string password);

    Task<TokenResponse?> RefreshTokenAsync(string refreshToken);

    Task UpdateUserProfile(UserModel userModel, Guid userId);
}