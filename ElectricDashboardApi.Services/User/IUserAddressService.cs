using ElectricDashboard.Models.User;
using ElectricDashboardApi.Dtos.User;

namespace ElectricDashboard.Services.User;

public interface IUserAddressService
{
    ValueTask<IReadOnlyCollection<ServiceAddress>> GetServiceAddresses(Guid userGuid);

    ValueTask<ServiceAddress?> AddAddress(Guid userGuid, ServiceAddressDto serviceAddress);
}
