using ElectricDashboardApi.Dtos.DataSources;

namespace ElectricDashboardApi.Data.Queries.DataSources;

public interface IGetElectricBillQuery
{
    Task<IReadOnlyCollection<ElectricBill>> GetElectricBills(
        Guid userId,
        Guid addressId,
        Guid? billId = null);
}