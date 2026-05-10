namespace ElectricDashboard.Services.User;

using ElectricDashboardApi.Dtos.User;
using ElectricDashboardApi.Infrastructure.Commands.User;
using ElectricDashboardApi.Infrastructure.Queries.DataSources;
using ElectricDashboardApi.Infrastructure.Queries.User;
using Microsoft.Extensions.Caching.Hybrid;

public class UserAddressService(
    HybridCache cache,
    IGetAddressExistsQuery getAddressExistsQuery,
    IGetUserAddressesQuery getUserAddressesQuery,
    IAddServiceAddressCommand addServiceAddressCommand,
    IUpdateServiceAddressCommand updateServiceAddressCommand,
    IDeleteServiceAddressCommand deleteServiceAddressCommand) : IUserAddressService
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

    public async Task<ServiceAddressDto?> AddAddress(Guid userGuid, ServiceAddressDto serviceAddress)
    {
        await cache.RemoveByTagAsync($"User:ServiceAddress:{userGuid}");

        return await addServiceAddressCommand.Execute(userGuid, serviceAddress);
    }

    public async Task<ServiceAddressDto?> UpdateServiceAddress(Guid userGuid, Guid addressGuid, ServiceAddressDto serviceAddress)
    {
        await cache.RemoveByTagAsync($"User:ServiceAddress:{userGuid}");

        return await updateServiceAddressCommand.Execute(userGuid, addressGuid, serviceAddress);
    }

    public async Task DeleteAddress(Guid userGuid, Guid addressGuid)
    {
        var addressExists = await getAddressExistsQuery.Execute(userGuid, addressGuid);

        if (addressExists)
        {
            await cache.RemoveByTagAsync($"User:ServiceAddress:{userGuid}");

            await deleteServiceAddressCommand.Execute(userGuid, addressGuid);
        }
    }
}
