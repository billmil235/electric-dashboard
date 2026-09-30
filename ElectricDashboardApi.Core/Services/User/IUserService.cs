using ElectricDashboardApi.Dtos.OAuth;
using ElectricDashboardApi.Dtos.User;
using System.Threading;
using System.Threading.Tasks;

namespace ElectricDashboard.Services.User;

public interface IUserService
{
    Task<bool> ExistsByEmailAsync(string emailAddress, CancellationToken cancellationToken);

    Task<CreateUserResult> CreateUserAsync(UserDto userModel, CancellationToken cancellationToken);

    Task<LoginResult> LoginAsync(string username, string password, CancellationToken cancellationToken);

    Task<RefreshTokenResponse?> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken);

    Task UpdateUserProfile(UserDto user, Guid userId);

    Task<UserDto> GetUserInformation(Guid userGuid);
}
