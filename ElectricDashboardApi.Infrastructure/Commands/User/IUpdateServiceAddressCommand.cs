using ElectricDashboardApi.Dtos.User;

namespace ElectricDashboardApi.Infrastructure.Commands.User;

public interface IUpdateServiceAddressCommand
{
    Task<ServiceAddressDto?> Execute(Guid userId, Guid addressId, ServiceAddressDto serviceAddress);
}
