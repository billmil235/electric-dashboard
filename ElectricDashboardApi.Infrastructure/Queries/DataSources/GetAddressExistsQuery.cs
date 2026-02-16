using Microsoft.EntityFrameworkCore;

namespace ElectricDashboardApi.Infrastructure.Queries.DataSources;

public class GetAddressExistsQuery(ElectricDashboardContext context) : IGetAddressExistsQuery
{
    public async Task<bool> ExecuteAsync(Guid userId, Guid addressId)
    {
        return await context.UserToServiceAddresses
            .AnyAsync(a => a.AddressId == addressId && a.UserId == userId);
    }
}
