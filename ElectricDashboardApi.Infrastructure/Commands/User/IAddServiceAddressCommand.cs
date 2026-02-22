using ElectricDashboardApi.Dtos.User;
using ElectricDashboardApi.Infrastructure.Entities;

namespace ElectricDashboardApi.Infrastructure.Commands.Users;

public interface IAddServiceAddressCommand
{
    Task<ServiceAddressDto?> AddServiceAddress(Guid userId, ServiceAddressDto serviceAddress);
}
