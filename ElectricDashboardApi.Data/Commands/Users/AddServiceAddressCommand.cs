using ElectricDashboard.Models.User;
using ServiceAddressEntity = ElectricDashboardApi.Data.Entities.ServiceAddress;

namespace ElectricDashboardApi.Data.Commands.Users;

public class AddServiceAddressCommand(ElectricDashboardContext context) : IAddServiceAddressCommand
{
    public async Task<ServiceAddress?> AddServiceAddress(Guid userId, ServiceAddress serviceAddress)
    {
        var entity = new ServiceAddressEntity
        {
            AddressId = Guid.CreateVersion7(),
            UserId = userId,
            AddressName = serviceAddress.AddressName,
            AddressLine1 = serviceAddress.AddressLine1,
            AddressLine2 = serviceAddress.AddressLine2,
            City = serviceAddress.City,
            State = serviceAddress.State,
            ZipCode = serviceAddress.ZipCode,
            Country = serviceAddress.Country
        };

        await context.ServiceAddresses.AddAsync(entity);
        await context.SaveChangesAsync();

        return serviceAddress with { AddressId = entity.AddressId };
    }
}