namespace ElectricDashboardApi.Infrastructure.Queries.DataSources;

public interface IGetAddressExistsQuery
{
    ValueTask<bool> Execute(Guid userId, Guid addressId);
}
