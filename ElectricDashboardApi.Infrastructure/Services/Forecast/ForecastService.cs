using ElectricDashboardApi.Dtos.Forecast;
using ElectricDashboardApi.Infrastructure.Commands.Forecast;
using ElectricDashboardApi.Infrastructure.Queries.Forecast;

namespace ElectricDashboardApi.Infrastructure.Services.Forecast;

public class ForecastService(IGetForecastQuery forecastQuery, ICacheForecastCommand cacheCommand)
    : IForecastService
{
    public async Task<ForecastResponse> GetForecastAsync(Guid addressId, Guid userId)
    {
        var forecast = await forecastQuery.GetForecastQueryAsync(addressId, userId).ConfigureAwait(false);
        if (forecast == null)
        {
            // Unauthorized or missing data
            throw new UnauthorizedAccessException();
        }

        // Ensure cache is up to date after calculation (query does not persist cache for newly computed forecasts)
        await cacheCommand.CacheForecastCommandAsync(
            addressId,
            forecast.ForecastYear,
            forecast.ForecastMonth,
            forecast.PredictedKwh,
            forecast.AlgorithmUsed,
            forecast.Confidence).ConfigureAwait(false);

        return forecast;
    }
}
