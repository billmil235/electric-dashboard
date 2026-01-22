using ElectricDashboardApi.Dtos.DataSources;
using ElectricBillModel = ElectricDashboard.Models.DataSources.ElectricBill;
using ElectricBillEntity = ElectricDashboardApi.Data.Entities.ElectricBill;

namespace ElectricDashboardApi.Data.Commands.DataSources;

public class AddElectricBillCommand(ElectricDashboardContext context) : IAddElectricBillCommand
{
    public async Task<ElectricBillModel?> AddElectricBill(Guid userId, ElectricBillDto electricBillDto)
    {
        var entity = new ElectricBillEntity
        {
            BillId = Guid.CreateVersion7(),
            AddressId = electricBillDto.AddressId!.Value,
            BilledAmount = electricBillDto.BilledAmount,
            PeriodStartDate = electricBillDto.PeriodStartDate.ToDateTime(TimeOnly.MinValue),
            PeriodEndDate = electricBillDto.PeriodEndDate.ToDateTime(TimeOnly.MinValue),
            SentBackKwh = electricBillDto.SentBackKwh,
            ConsumptionKwh = electricBillDto.ConsumptionKwh,
            UnitPrice = electricBillDto.UnitPrice
        };

        await context.ElectricBills.AddAsync(entity);
        await context.SaveChangesAsync();

        return entity.ToModel();
    }
}