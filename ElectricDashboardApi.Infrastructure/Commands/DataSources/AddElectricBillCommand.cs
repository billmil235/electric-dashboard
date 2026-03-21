using ElectricDashboardApi.Dtos.DataSources;
using ElectricDashboardApi.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace ElectricDashboardApi.Infrastructure.Commands.DataSources;

public class AddElectricBillCommand(ElectricDashboardContext context) : IAddElectricBillCommand
{
    public async Task<ElectricBill?> AddElectricBill(Guid userId, ElectricBillDto electricBillDto, Guid? billGuid = null)
    {
        ElectricBill? entity;

        if (billGuid.HasValue)
        {
            entity = await context.ElectricBills
                .FirstAsync(x => x.BillId == billGuid)
                .ConfigureAwait(false);
        }
        else
        {
            entity = new ElectricBill
            {
                BillId = Guid.CreateVersion7(),
                AddressId = electricBillDto.AddressId!.Value
            };
        }

        entity.BilledAmount = electricBillDto.BilledAmount;
        entity.PeriodStartDate = electricBillDto.PeriodStartDate.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
        entity.PeriodEndDate = electricBillDto.PeriodEndDate.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
        entity.SentBackKwh = electricBillDto.SentBackKwh;
        entity.ConsumptionKwh = electricBillDto.ConsumptionKwh;
        entity.UnitPrice = electricBillDto.UnitPrice;

        await context.ElectricBills.AddAsync(entity).ConfigureAwait(false);
        await context.SaveChangesAsync().ConfigureAwait(false);

        return entity;
    }
}
