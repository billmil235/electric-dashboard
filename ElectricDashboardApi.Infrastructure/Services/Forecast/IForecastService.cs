using ElectricDashboardApi.Dtos.Forecast;

namespace ElectricDashboardApi.Infrastructure.Services.Forecast;

public interface IForecastService
{
    Task<ForecastResponse> GetForecastAsync(Guid addressId, Guid userId);
}
