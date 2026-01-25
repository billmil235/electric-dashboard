using ElectricDashboard.Models.User;
using Microsoft.EntityFrameworkCore;

namespace ElectricDashboardApi.Data.Queries.User;

public class GetUserAddressesQuery(ElectricDashboardContext context) : IGetUserAddressesQuery
{
    public async Task<IReadOnlyCollection<ServiceAddress>> GetUserAddresses(
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

        var userAddresses = await query
            .Select(usa => usa.ServiceAddress)
            .ToListAsync();

        return userAddresses.Select(a => new ServiceAddress()
        {
            AddressId = a.AddressId,
            AddressLine1 = a.AddressLine1,
            AddressLine2 = a.AddressLine2,
            AddressName = a.AddressName,
            City = a.City,
            State = a.State,
            ZipCode = a.ZipCode,
            Country = a.Country,
            IsCommercial = a.IsCommercial
        }).ToList();
    }
}