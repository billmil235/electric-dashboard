using ElectricDashboard.Models.DataSources;
using Microsoft.EntityFrameworkCore;

namespace ElectricDashboardApi.Data.Queries.DataSources;

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

        var bills = await query.ToListAsync();
        return bills.Select(bill => bill.ToModel()).ToList();
    }
}
