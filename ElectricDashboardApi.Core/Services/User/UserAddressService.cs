using ElectricDashboardApi.Infrastructure.Commands.Users;
using ElectricDashboardApi.Infrastructure.Queries.DataSources;
using ElectricDashboardApi.Infrastructure.Queries.User;
using ElectricDashboardApi.Dtos.User;
using Microsoft.Extensions.Caching.Hybrid;

namespace ElectricDashboard.Services.User;

public class UserAddressService(
    HybridCache cache,
    IGetAddressExistsQuery getAddressExistsQuery,
    IGetUserAddressesQuery getUserAddressesQuery,
    IAddServiceAddressCommand addServiceAddressCommand) : IUserAddressService
{

    public async ValueTask<IReadOnlyCollection<ServiceAddressDto>> GetServiceAddresses(Guid userGuid)
    {
        return await cache.GetOrCreateAsync(
            $"User:ServiceAddress:{userGuid}",
            Factory,
            null,
            null,
            CancellationToken.None
        );

        async ValueTask<IReadOnlyCollection<ServiceAddressDto>> Factory(CancellationToken cancel) =>
            await getUserAddressesQuery.GetUserAddresses(userGuid);
    }

    public async ValueTask<ServiceAddressDto?> AddAddress(Guid userGuid, ServiceAddressDto serviceAddress)
    {
        // Invalidate cache for this user Guid
        await cache.RemoveByTagAsync($"User:ServiceAddress:{userGuid}");

        // Create address in the database
        return await addServiceAddressCommand.AddServiceAddress(userGuid, serviceAddress);
    }

    public async Task DeleteAddress(Guid userGuid, Guid addressGuid)
    {
        var addressExists = await getAddressExistsQuery.ExecuteAsync(userGuid, addressGuid);

        if (addressExists)
        {
            // Invalidate cache for this user Guid
            await cache.RemoveByTagAsync($"User:ServiceAddress:{userGuid}");

            // Delete the address from the database
        }
    }

}
