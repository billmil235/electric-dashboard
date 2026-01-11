using UserModel = ElectricDashboard.Models.User.User;

namespace ElectricDashboardApi.Data.Commands.User;

public interface IUpdateProfileCommand
{
    Task Execute(UserModel userModel, Guid userId);
}