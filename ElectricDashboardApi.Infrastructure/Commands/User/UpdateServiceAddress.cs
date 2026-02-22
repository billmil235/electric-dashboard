namespace ElectricDashboardApi.Infrastructure.Commands.User;

using ElectricDashboardApi.Dtos.User;
using ElectricDashboardApi.Infrastructure.Entities;

public class UpdateServiceAddress(ElectricDashboardContext context) : IUpdateServiceAddress
{
    public async Task<ServiceAddressDto?> Execute(Guid userId, Guid addressId, ServiceAddressDto serviceAddress)
    {
        var address = await context.ServiceAddresses.FindAsync(addressId);

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

        await context.SaveChangesAsync();

        return serviceAddress;
    }
}
