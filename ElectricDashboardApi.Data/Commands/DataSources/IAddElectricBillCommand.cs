using ElectricDashboardApi.Dtos.DataSources;

namespace ElectricDashboardApi.Data.Commands.DataSources;

public interface IAddElectricBillCommand
{
    Task<ElectricBill?> AddElectricBill(Guid userId, ElectricBill electricBill);
}