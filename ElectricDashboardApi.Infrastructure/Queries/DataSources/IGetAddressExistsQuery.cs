namespace ElectricDashboardApi.Infrastructure.Queries.DataSources;

public interface IGetAddressExistsQuery
{
    ValueTask<bool> ExecuteAsync(Guid userId, Guid addressId);
}
