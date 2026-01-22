using ElectricDashboardApi.Dtos.DataSources;
using ElectricBillModel = ElectricDashboard.Models.DataSources.ElectricBill;

namespace ElectricDashboardApi.Data.Commands.DataSources;

public interface IAddElectricBillCommand
{
    Task<ElectricBillModel?> AddElectricBill(Guid userId, ElectricBillDto electricBillDto);
}