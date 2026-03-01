using ElectricDashboardApi.Dtos.Forecast;
using ElectricDashboardApi.Infrastructure.Algorithms;
using ElectricDashboardApi.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace ElectricDashboardApi.Infrastructure.Services.Forecast
{
    public class ForecastService(ElectricDashboardContext context) : IForecastService
    {
        public async Task<ForecastResponse> GetForecastAsync(Guid addressId, Guid userId)
        {
            // Verify ownership
            var owns = await context.UserToServiceAddresses
                .AnyAsync(ua => ua.UserId == userId && ua.AddressId == addressId);
            if (!owns) throw new UnauthorizedAccessException();

            // Determine next month
            var now = DateTime.UtcNow;
            var nextMonth = now.Month == 12 ? 1 : now.Month + 1;
            var nextYear = now.Month == 12 ? now.Year + 1 : now.Year;

            // Cache lookup
            var cached = await context.ForecastCaches
                .Where(fc => fc.AddressId == addressId && fc.ForecastYear == nextYear && fc.ForecastMonth == nextMonth)
                .FirstOrDefaultAsync();

            if (cached != null && cached.CachedAt > DateTime.UtcNow.AddHours(-24))
            {
                return new ForecastResponse
                {
                    AddressId = addressId,
                    ForecastMonth = nextMonth,
                    ForecastYear = nextYear,
                    PredictedKwh = cached.PredictedKwh,
                    AlgorithmUsed = cached.AlgorithmUsed,
                    Confidence = 1.0m // cached value assumed reliable
                };
            }

            // Pull historical bills and aggregate
            var bills = await context.ElectricBills
                .Where(b => b.AddressId == addressId)
                .ToListAsync();

            // Build monthly totals
            var monthly = bills.GroupBy(b => new { b.PeriodEndDate.Year, b.PeriodEndDate.Month })
                .Select(g => new MonthlyBill { Year = g.Key.Year, Month = g.Key.Month, Total = g.Sum(b => b.ConsumptionKwh) })
                .OrderBy(m => m.Year).ThenBy(m => m.Month)
                .ToList();

            // Forward‑fill missing months up to current month
            if (monthly.Count > 0)
            {
                var last = monthly.Last();
                var startYear = monthly.First().Year;
                var startMonth = monthly.First().Month;
                var monthsSinceLast = (nextYear - startYear) * 12 + (nextMonth - last.Month); // naive
                // Not needed for forecast; algorithm will handle missing via last known data
            }

            // Choose algorithm
            double maeHolt;
            var predLinear = LinearTrendPredictor.Predict(monthly, out var maeLinear);
            var predHolt = HoltWintersPredictor.Predict(monthly, out maeHolt);

            var algorithmUsed = string.Empty;
            double predicted;
            // We compare MAE relative to mean amount
            var mean = monthly.Select(m => m.Total).Average();
            var linearWithin = (maeLinear / (double)mean <= 0.10) && maeLinear <= maeHolt;
            var holtWithin = (maeHolt / (double)mean <= 0.10) && maeHolt < maeLinear;

            if (linearWithin)
            {
                algorithmUsed = "LinearTrend"; predicted = predLinear;
            }
            else if (holtWithin)
            {
                algorithmUsed = "HoltWinters"; predicted = predHolt;
            }
            else
            {
                algorithmUsed = "LinearTrend"; predicted = predLinear;
            }

            // Cache result
            var newCache = new ForecastCache
            {
                AddressId = addressId,
                ForecastYear = nextYear,
                ForecastMonth = nextMonth,
                PredictedKwh = (decimal)predicted,
                AlgorithmUsed = algorithmUsed,
                CachedAt = DateTime.UtcNow
            };
            context.ForecastCaches.Add(newCache);
            await context.SaveChangesAsync();

            return new ForecastResponse
            {
                AddressId = addressId,
                ForecastMonth = nextMonth,
                ForecastYear = nextYear,
                PredictedKwh = (decimal)predicted,
                AlgorithmUsed = algorithmUsed,
                Confidence = 1 - ((decimal)Math.Min(maeLinear, maeHolt) / mean) // crude confidence
            };
        }
    }
}
