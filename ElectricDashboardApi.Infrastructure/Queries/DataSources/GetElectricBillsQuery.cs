using ElectricDashboardApi.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace ElectricDashboardApi.Infrastructure.Queries.DataSources;

public class GetElectricBillsQuery(ElectricDashboardContext context) : IGetElectricBillQuery
{
    public async ValueTask<IReadOnlyCollection<ElectricBill>> Execute(
        Guid userId,
        Guid addressId,
        Guid? billId = null)
    {
        var owns = await context.UserToServiceAddresses
            .AnyAsync(ua => ua.UserId == userId && ua.AddressId == addressId)
            .ConfigureAwait(false);
        
        if (!owns)
        {
            return [];
        }

        var query = context.ElectricBills
            .Where(bill => bill.AddressId == addressId)
            .AsNoTracking();

        if (billId.HasValue)
        {
            query = query.Where(bill => bill.BillId == billId);
        }

        return await query.ToListAsync().ConfigureAwait(false);
    }
}
