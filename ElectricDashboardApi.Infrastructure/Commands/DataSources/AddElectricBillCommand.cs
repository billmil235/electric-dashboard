using ElectricDashboardApi.Dtos.DataSources;
using ElectricDashboardApi.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace ElectricDashboardApi.Infrastructure.Commands.DataSources;

public class AddElectricBillCommand(ElectricDashboardContext context) : IAddElectricBillCommand
{
    public async Task<ElectricBill?> AddElectricBill(Guid userId, ElectricBillDto electricBillDto, Guid? billGuid = null)
    {
        ElectricBill? entity;
        var billId = billGuid ?? Guid.CreateVersion7();

        if (billGuid.HasValue)
        {
            entity = await context.ElectricBills
                .FirstAsync(x => x.BillId == billGuid)
                .ConfigureAwait(false);
        }
        else
        {
            entity = new ElectricBill();
        }

        entity.BillId = billId;
        entity.AddressId = electricBillDto.AddressId!.Value;
        entity.PeriodStartDate = electricBillDto.PeriodStartDate.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
        entity.PeriodEndDate = electricBillDto.PeriodEndDate.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
        entity.ConsumptionKwh = electricBillDto.ConsumptionKwh;
        entity.SentBackKwh = electricBillDto.SentBackKwh;
        entity.UnitPrice = electricBillDto.UnitPrice;
        entity.BilledAmount = electricBillDto.BilledAmount;

        if (!billGuid.HasValue)
        {
            await context.ElectricBills.AddAsync(entity).ConfigureAwait(false);
        }

        await context.SaveChangesAsync().ConfigureAwait(false);

        return entity;
    }
}
