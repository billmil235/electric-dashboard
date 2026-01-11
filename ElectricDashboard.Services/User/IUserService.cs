using ElectricDashboard.Models.Keycloak;

namespace ElectricDashboard.Services.User;

public interface IUserService
{
    Task CreateUserAsync(Models.User.User userModel);
    
    Task<string> LoginAsync(string username, string password);

    Task<TokenResponse?> RefreshTokenAsync(string refreshToken);
}