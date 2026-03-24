using ElectricDashboardApi.Dtos.Forecast;

namespace ElectricDashboardApi.Infrastructure.Queries.Forecast;

public interface IGetForecastQuery
{
    Task<ForecastResponse?> GetForecastQueryAsync(Guid addressId, Guid userId);
}