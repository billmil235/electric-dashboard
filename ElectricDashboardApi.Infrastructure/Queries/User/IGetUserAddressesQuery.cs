using ElectricDashboardApi.Dtos.User;
using ElectricDashboardApi.Infrastructure.Entities;

namespace ElectricDashboardApi.Infrastructure.Queries.User;

public interface IGetUserAddressesQuery
{
    Task<IReadOnlyCollection<ServiceAddressDto>> GetUserAddresses(
        Guid userId,
        Guid? addressId = null);
}
