using ElectricDashboard.Models.DataSources;
using ElectricBillEntity = ElectricDashboardApi.Infrastructure.Entities.ElectricBill;

namespace ElectricDashboardApi.Mappers;

public static class ElectricBillMapper
{
    public static ElectricBill ToModel(ElectricBillEntity electricBill)
    {
        return new ElectricBill()
        {
            AddressId = electricBill.AddressId,
            BilledAmount = electricBill.BilledAmount,
            PeriodStartDate = DateOnly.FromDateTime(electricBill.PeriodStartDate),
            PeriodEndDate = DateOnly.FromDateTime(electricBill.PeriodEndDate),
            SentBackKwh = electricBill.SentBackKwh,
            UnitPrice = electricBill.UnitPrice,
            ConsumptionKwh = electricBill.ConsumptionKwh,
            Note = electricBill.Note
        };
    }
}
