using ElectricDashboardApi.Dtos.User;

namespace ElectricDashboardApi.Infrastructure.Commands.User;

public interface IUpdateProfileCommand
{
    Task Execute(UserDto userModel, Guid userId);
}
