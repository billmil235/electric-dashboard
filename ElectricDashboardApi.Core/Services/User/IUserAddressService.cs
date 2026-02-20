using ElectricDashboardApi.Dtos.User;
using ElectricDashboardApi.Infrastructure.Entities;

namespace ElectricDashboard.Services.User;

public interface IUserAddressService
{
    ValueTask<IReadOnlyCollection<ServiceAddressDto>> GetServiceAddresses(Guid userGuid);

    Task<ServiceAddressDto?> AddAddress(Guid userGuid, ServiceAddressDto serviceAddress);
}
