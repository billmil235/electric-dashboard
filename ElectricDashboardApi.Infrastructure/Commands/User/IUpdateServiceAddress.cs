using ElectricDashboardApi.Dtos.User;

namespace ElectricDashboardApi.Infrastructure.Commands.User;

public interface IUpdateServiceAddress
{
    Task<ServiceAddressDto?> Execute(Guid userId, Guid addressId, ServiceAddressDto serviceAddress);
}
