using ElectricDashboard.Models.User;

namespace ElectricDashboardApi.Data.Commands.Users;

public interface IAddServiceAddressCommand
{
    Task<ServiceAddress?> AddServiceAddress(Guid userId, ServiceAddress serviceAddress);
}
