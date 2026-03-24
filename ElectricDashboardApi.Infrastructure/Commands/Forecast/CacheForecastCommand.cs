using ElectricDashboardApi.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace ElectricDashboardApi.Infrastructure.Commands.Forecast;

public class CacheForecastCommand(ElectricDashboardContext context) : ICacheForecastCommand
{
    public async Task CacheForecastCommandAsync(Guid addressId, int forecastYear, int forecastMonth, decimal predictedKwh, string algorithmUsed, decimal confidence)
    {
        var existingCache = await context.ForecastCaches
            .FirstOrDefaultAsync(fc => fc.AddressId == addressId && fc.ForecastYear == forecastYear && fc.ForecastMonth == forecastMonth)
            .ConfigureAwait(false);

        if (existingCache == null)
        {
            existingCache = new ForecastCache
            {
                AddressId = addressId,
                ForecastYear = forecastYear,
                ForecastMonth = forecastMonth,
                PredictedKwh = predictedKwh,
                AlgorithmUsed = algorithmUsed,
                CachedAt = DateTime.UtcNow,
                Confidence = confidence
            };
            context.ForecastCaches.Add(existingCache);
        }
        else
        {
            existingCache.PredictedKwh = predictedKwh;
            existingCache.AlgorithmUsed = algorithmUsed;
            existingCache.CachedAt = DateTime.UtcNow;
            existingCache.Confidence = confidence;
        }

        await context.SaveChangesAsync().ConfigureAwait(false);
    }
}
