namespace ElectricDashboardApi.Infrastructure.Queries.DataSources;

public interface IGetAddressExistsQuery
{
    Task<bool> ExecuteAsync(Guid userId, Guid addressId);
}
