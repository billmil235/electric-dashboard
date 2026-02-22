using ElectricDashboardApi.Dtos.User;
using ElectricDashboardApi.Infrastructure.Entities;

namespace ElectricDashboardApi.Infrastructure.Commands.User;

public class AddServiceAddressCommand(ElectricDashboardContext context) : IAddServiceAddressCommand
{
    public async Task<ServiceAddressDto?> Execute(Guid userId, ServiceAddressDto serviceAddress)
    {
        var entity = new ServiceAddress
        {
            AddressId = Guid.CreateVersion7(),
            AddressName = serviceAddress.AddressName,
            AddressLine1 = serviceAddress.AddressLine1,
            AddressLine2 = serviceAddress.AddressLine2,
            City = serviceAddress.City,
            State = serviceAddress.State,
            ZipCode = serviceAddress.ZipCode,
            Country = serviceAddress.Country,
            IsCommercial = serviceAddress.IsCommercial
        };

        await context.ServiceAddresses.AddAsync(entity);

        var userToServiceAddress = new UserToServiceAddress
        {
            UserId = userId,
            AddressId = entity.AddressId
        };

        await context.UserToServiceAddresses.AddAsync(userToServiceAddress);
        await context.SaveChangesAsync();

        return serviceAddress with { AddressId = entity.AddressId };
    }
}
