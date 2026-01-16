using ElectricDashboard.Models.User;

namespace ElectricDashboardApi.Data.Queries.User;

public interface IGetUserAddressesQuery
{
    Task<IReadOnlyCollection<ServiceAddress>> GetUserAddresses(
        Guid userId,
        Guid? addressId = null);
}
