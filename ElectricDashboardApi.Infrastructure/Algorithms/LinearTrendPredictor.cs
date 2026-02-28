using System;
using System.Collections.Generic;
using System.Linq;

namespace ElectricDashboardApi.Infrastructure.Algorithms
{
    public static class LinearTrendPredictor
    {
        public static double Predict(List<MonthlyBill> bills, out double mae)
        {
            mae = double.NaN;
            
            if (bills == null || !bills.Any()) 
            {
                return 0; 
            }
            
            var n = bills.Count;

            // X is 1..n
            var xs = Enumerable.Range(1, n).Select(i => (double)i).ToList();
            var ys = bills.Select(b => (double)b.Total).ToList();
            var sumX = xs.Sum();
            var sumY = ys.Sum();
            var sumXY = xs.Zip(ys, (x, y) => x * y).Sum();
            var sumX2 = xs.Select(x => x * x).Sum();
            var denominator = n * sumX2 - sumX * sumX;
            
            if (denominator == 0) 
            {
                return 0;
            }

            var a = (n * sumXY - sumX * sumY) / denominator;
            var b = (sumY - a * sumX) / n;
            // Predict next point
            var predicted = a * (n + 1) + b;
            // Compute MAE on last k months (use 3 or all if fewer)
            var k = Math.Min(3, n - 1);
            var recent = bills.Skip(n - k).ToList();
            var recentX = Enumerable.Range(n - k + 1, k).Select(i => (double)i).ToList();
            var errors = recentX.Zip(recent, (x, bill) => Math.Abs(a * x + b - (double)bill.Total)).ToList();
            mae = errors.Sum() / k;
            return predicted;
        }
    }

    public class MonthlyBill
    {
        public int Year { get; set; }

        public int Month { get; set; }
        
        public decimal Total { get; set; }
    }
}
