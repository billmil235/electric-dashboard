using ElectricDashboardApi.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace ElectricDashboardApi.Infrastructure.Queries.DataSources;

public class GetElectricBillsQuery(ElectricDashboardContext context) : IGetElectricBillQuery
{
    public async Task<IReadOnlyCollection<ElectricBill>> GetElectricBills(
        Guid userId,
        Guid addressId,
        Guid? billId = null)
    {
        var query = context.ElectricBills.Where(bill => bill.AddressId == addressId);

        if (billId.HasValue)
        {
            query = query.Where(bill => bill.BillId == billId);
        }

        return await query.ToListAsync();
    }
}
