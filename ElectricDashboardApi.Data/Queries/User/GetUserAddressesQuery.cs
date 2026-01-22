using ElectricDashboard.Models.User;
using Microsoft.EntityFrameworkCore;

namespace ElectricDashboardApi.Data.Queries.User;

public class GetUserAddressesQuery(ElectricDashboardContext context) : IGetUserAddressesQuery
{
    public async Task<IReadOnlyCollection<ServiceAddress>> GetUserAddresses(
        Guid userId,
        Guid? addressId = null)
    {
        var query = context.ServiceAddresses
            .Where(a => a.UserId == userId);

        if (addressId.HasValue)
        {
            query = query.Where(a => a.AddressId == addressId.Value);
        }

        var addresses = await query.ToListAsync();

        return addresses.Select(a => new ServiceAddress()
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