using System;
using System.Collections.Generic;
using System.Linq;

namespace ElectricDashboardApi.Infrastructure.Algorithms
{
    public static class HoltWintersPredictor
    {
        // Simple additive Holt‑Winters
        public static double Predict(List<MonthlyBill> bills, out double mae, double alpha = 0.2, double beta = 0.2, double gamma = 0.2)
        {
            mae = double.NaN;
            if (bills == null || bills.Count == 0)
            {
                return 0;
            }
            var n = bills.Count;
            var period = 12;
            if (n < period) return 0;
            var l = new double[n];
            var b = new double[n];
            var s = new double[n];
            double initialLevel = bills.Take(period).Average(bc => (double)bc.Total);
            l[0] = initialLevel;
            double initialTrend = 0;
            if (n >= period * 2)
            {
                double sum = 0;
                for (int i = 0; i < period; i++)
                {
                    sum += (double)bills[i + period].Total - (double)bills[i].Total;
                }
                initialTrend = sum / (period * period);
            }
            b[0] = initialTrend;
            for (int i = 0; i < period; i++)
            {
                s[i] = (double)bills[i].Total - initialLevel;
            }
            for (int i = period; i < n; i++)
            {
                var value = (double)bills[i].Total;
                var prevL = l[i - 1];
                var prevB = b[i - 1];
                var prevS = s[i - period];
                l[i] = alpha * value + (1 - alpha) * (prevL + prevB);
                b[i] = beta * (l[i] - prevL) + (1 - beta) * prevB;
                s[i] = gamma * value + (1 - gamma) * prevS;
            }
            double forecast;
            if (n == period)
            {
                forecast = l[0] + b[0] + s[0];
            }
            else
            {
                forecast = l[n - 1] + b[n - 1] + s[n - 1];
            }
            var k = Math.Min(3, n - 1);
            var errors = new List<double>();
            for (int i = n - k; i < n; i++)
            {
                double est = l[i] + b[i] + s[i];
                errors.Add(Math.Abs(est - (double)bills[i].Total));
            }
            mae = errors.Count > 0 ? errors.Average() : 0;
            return forecast;
        }
    }
}
