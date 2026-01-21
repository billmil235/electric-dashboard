using ElectricDashboardApi.Dtos.DataSources;
using Microsoft.EntityFrameworkCore;

namespace ElectricDashboardApi.Data.Queries.DataSources;

public class GetElectricBillsQuery(ElectricDashboardContext context) : IGetElectricBillQuery
{
    public async Task<IReadOnlyCollection<ElectricBill>> GetElectricBills(
        Guid userId, 
        Guid addressId,
        Guid? billId = null)
    {
        var bills = await context.ElectricBills.Where(bill => bill.AddressId == addressId).ToListAsync();

        return bills.Select(bill => new ElectricBill()
        {
            AddressId = bill.AddressId,
            BilledAmount = bill.BilledAmount,
            PeriodEndDate = DateOnly.FromDateTime(bill.PeriodEndDate),
            PeriodStartDate = DateOnly.FromDateTime(bill.PeriodStartDate),
            SentBackKwh = bill.SentBackKwh,
            ConsumptionKwh = bill.ConsumptionKwh,
            UnitPrice = bill.UnitPrice,
        }).ToList();
    }
}