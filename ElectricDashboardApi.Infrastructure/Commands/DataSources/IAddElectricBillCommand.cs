using ElectricDashboardApi.Dtos.DataSources;
using ElectricDashboardApi.Infrastructure.Entities;

namespace ElectricDashboardApi.Infrastructure.Commands.DataSources;

public interface IAddElectricBillCommand
{
    Task<ElectricBill?> AddElectricBill(Guid userId, ElectricBillDto electricBillDto);
}
