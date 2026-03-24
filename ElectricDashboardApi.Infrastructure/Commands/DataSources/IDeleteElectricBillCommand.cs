using ElectricDashboardApi.Infrastructure.Entities;

namespace ElectricDashboardApi.Infrastructure.Commands.DataSources;

public interface IDeleteElectricBillCommand
{
    Task<bool> DeleteElectricBill(Guid userId, Guid billId);
}
