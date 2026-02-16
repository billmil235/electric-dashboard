using ElectricDashboardApi.Dtos.DataSources;
using ElectricDashboardApi.Infrastructure.Entities;

namespace ElectricDashboardApi.Infrastructure.Commands.DataSources;

public class AddElectricBillCommand(ElectricDashboardContext context) : IAddElectricBillCommand
{
    public async Task<ElectricBill?> AddElectricBill(Guid userId, ElectricBillDto electricBillDto)
    {
        var entity = new ElectricBill
        {
            BillId = Guid.CreateVersion7(),
            AddressId = electricBillDto.AddressId!.Value,
            BilledAmount = electricBillDto.BilledAmount,
            PeriodStartDate = electricBillDto.PeriodStartDate.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc),
            PeriodEndDate = electricBillDto.PeriodEndDate.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc),
            SentBackKwh = electricBillDto.SentBackKwh,
            ConsumptionKwh = electricBillDto.ConsumptionKwh,
            UnitPrice = electricBillDto.UnitPrice
        };

        await context.ElectricBills.AddAsync(entity);
        await context.SaveChangesAsync();

        return entity;
    }
}
