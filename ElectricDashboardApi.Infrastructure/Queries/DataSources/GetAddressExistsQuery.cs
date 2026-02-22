using Microsoft.EntityFrameworkCore;

namespace ElectricDashboardApi.Infrastructure.Queries.DataSources;

public class GetAddressExistsQuery(ElectricDashboardContext context) : IGetAddressExistsQuery
{
    public async ValueTask<bool> Execute(Guid userId, Guid addressId)
    {
        return await context.UserToServiceAddresses
            .AnyAsync(a => a.AddressId == addressId && a.UserId == userId);
    }
}
