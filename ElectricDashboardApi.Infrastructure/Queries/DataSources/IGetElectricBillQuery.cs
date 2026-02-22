using ElectricDashboardApi.Infrastructure.Entities;

namespace ElectricDashboardApi.Infrastructure.Queries.DataSources;

public interface IGetElectricBillQuery
{
    ValueTask<IReadOnlyCollection<ElectricBill>> Execute(
        Guid userId,
        Guid addressId,
        Guid? billId = null);
}
