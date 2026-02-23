using ElectricDashboardApi.Dtos.User;
using Microsoft.EntityFrameworkCore;

namespace ElectricDashboardApi.Infrastructure.Queries.User;

public class GetUserAddressesQuery(ElectricDashboardContext context) : IGetUserAddressesQuery
{
    public async Task<IReadOnlyCollection<ServiceAddressDto>> GetUserAddresses(
        Guid userId,
        Guid? addressId = null)
    {
        var query = context.UserToServiceAddresses
            .Include(usa => usa.ServiceAddress)
            .Where(usa => usa.UserId == userId);

        if (addressId.HasValue)
        {
            query = query.Where(usa => usa.AddressId == addressId.Value);
        }

        return await query
            .Select(usa => new ServiceAddressDto
            {
                AddressId = usa.AddressId,
                AddressName = usa.ServiceAddress.AddressName,
                AddressLine1 = usa.ServiceAddress.AddressLine1,
                AddressLine2 = usa.ServiceAddress.AddressLine2,
                City = usa.ServiceAddress.City,
                State = usa.ServiceAddress.State,
                ZipCode = usa.ServiceAddress.ZipCode,
                Country = usa.ServiceAddress.Country,
                IsCommercial = usa.ServiceAddress.IsCommercial,
                ElectricCompanyId = usa.ServiceAddress.ElectricCompanyId
            })
            .ToListAsync();
    }
}
