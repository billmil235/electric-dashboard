using ElectricDashboardApi.Infrastructure.Entities;

namespace ElectricDashboardApi.Infrastructure.Queries.DataSources;

public interface IGetElectricBillQuery
{
    Task<IReadOnlyCollection<ElectricBill>> GetElectricBills(
        Guid userId,
        Guid addressId,
        Guid? billId = null);
}
