using ElectricDashboardApi.Data.Entities;
using ElectricDashboardApi.Dtos.User;
using ServiceAddress = ElectricDashboard.Models.User.ServiceAddress;
using ServiceAddressEntity = ElectricDashboardApi.Data.Entities.ServiceAddress;

namespace ElectricDashboardApi.Data.Commands.Users;

public class AddServiceAddressCommand(ElectricDashboardContext context) : IAddServiceAddressCommand
{
    public async Task<ServiceAddress?> AddServiceAddress(Guid userId, ServiceAddressDto serviceAddress)
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
            Country = serviceAddress.Country,
            IsCommercial = serviceAddress.IsCommercial
        };

        await context.ServiceAddresses.AddAsync(entity);

        // Add entry to UserToServiceAddress table
        var userToServiceAddress = new UserToServiceAddress
        {
            UserId = userId,
            AddressId = entity.AddressId
        };

        await context.UserToServiceAddresses.AddAsync(userToServiceAddress);
        await context.SaveChangesAsync();

        return entity.ToModel();
    }
}