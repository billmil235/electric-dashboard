using ElectricDashboardApi.Infrastructure.Entities;

namespace ElectricDashboardApi.Infrastructure.Commands.Forecast;

public interface ICacheForecastCommand
{
    Task CacheForecastCommandAsync(Guid addressId, int forecastYear, int forecastMonth, decimal predictedKwh, string algorithmUsed, decimal confidence);
}