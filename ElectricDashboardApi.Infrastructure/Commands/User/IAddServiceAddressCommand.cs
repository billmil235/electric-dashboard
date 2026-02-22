using ElectricDashboardApi.Dtos.User;

namespace ElectricDashboardApi.Infrastructure.Commands.User;

public interface IAddServiceAddressCommand
{
    Task<ServiceAddressDto?> Execute(Guid userId, ServiceAddressDto serviceAddress);
}
