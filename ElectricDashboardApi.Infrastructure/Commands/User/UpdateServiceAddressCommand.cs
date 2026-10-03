namespace ElectricDashboardApi.Infrastructure.Commands.User;

using ElectricDashboardApi.Dtos.User;
using ElectricDashboardApi.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

public class UpdateServiceAddressCommand(ElectricDashboardContext context) : IUpdateServiceAddressCommand
{
    public async Task<ServiceAddressDto?> Execute(Guid userId, Guid addressId, ServiceAddressDto serviceAddress)
    {
        // Verify that the user has access to this address
        var hasAccess = await context.UserToServiceAddresses
            .AnyAsync(usa => usa.UserId == userId && usa.AddressId == addressId)
            .ConfigureAwait(false);

        if (!hasAccess)
        {
            return null;
        }

        var address = await context.ServiceAddresses
            .FindAsync(addressId)
            .ConfigureAwait(false);

        if (address == null)
        {
            return null;
        }

        address.AddressName = serviceAddress.AddressName;
        address.AddressLine1 = serviceAddress.AddressLine1;
        address.AddressLine2 = serviceAddress.AddressLine2;
        address.City = serviceAddress.City;
        address.State = serviceAddress.State;
        address.ZipCode = serviceAddress.ZipCode;
        address.Country = serviceAddress.Country;
        address.IsCommercial = serviceAddress.IsCommercial;

        await context.SaveChangesAsync().ConfigureAwait(false);

        return serviceAddress;
    }
}
