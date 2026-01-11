using ElectricDashboard.Models.Keycloak;

namespace ElectricDashboard.Services.User;

public interface IUserService
{
    Task CreateUserAsync(string email, string password);
    
    Task<string> LoginAsync(string username, string password);

    Task<TokenResponse?> RefreshTokenAsync(string refreshToken);
}