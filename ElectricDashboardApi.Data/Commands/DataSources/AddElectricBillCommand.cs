using ElectricDashboardApi.Dtos.DataSources;
using ElectricBillEntity = ElectricDashboardApi.Data.Entities.ElectricBill;

namespace ElectricDashboardApi.Data.Commands.DataSources;

public class AddElectricBillCommand(ElectricDashboardContext context) : IAddElectricBillCommand
{
    public async Task<ElectricBill?> AddElectricBill(Guid userId, ElectricBill electricBill)
    {
        var entity = new ElectricBillEntity
        {
            BillId = Guid.CreateVersion7(),
            AddressId = electricBill.AddressId!.Value,
            BilledAmount = electricBill.BilledAmount,
            PeriodStartDate = electricBill.PeriodStartDate.ToDateTime(TimeOnly.MinValue),
            PeriodEndDate = electricBill.PeriodEndDate.ToDateTime(TimeOnly.MinValue),
            SentBackKwh = electricBill.SentBackKwh,
            ConsumptionKwh = electricBill.ConsumptionKwh
        };

        await context.ElectricBills.AddAsync(entity);
        await context.SaveChangesAsync();

        return electricBill with { BillId = entity.BillId };
    }
}