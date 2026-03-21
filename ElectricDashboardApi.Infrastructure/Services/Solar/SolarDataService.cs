using System.Globalization;
using System.Text;
using ElectricDashboardApi.Dtos.Csv;
using ElectricDashboardApi.Dtos.DataSources;

namespace ElectricDashboardApi.Infrastructure.Services.Solar
{
    public interface ISolarDataService
    {
        Task<CsvHeaderDto> GetCsvHeader(Stream fileStream, string contentType);

        Task<List<SolarDataDto>> ParseCsvWithColumns(
            Stream fileStream,
            string contentType,
            string dateColumnName,
            string valueColumnName
        );
    }

    public class SolarDataService : ISolarDataService
    {
        public async Task<CsvHeaderDto> GetCsvHeader(Stream fileStream, string contentType)
        {
            using var reader = new StreamReader(fileStream, Encoding.UTF8, true, 1024, true);

            var headerLine = await reader.ReadLineAsync();
            if (headerLine == null)
            {
                return new CsvHeaderDto { Headers = [] };
            }

            var headers = ParseCsvLine(headerLine);

            return new CsvHeaderDto { Headers = headers };
        }

        public async Task<List<SolarDataDto>> ParseCsvWithColumns(
            Stream fileStream,
            string contentType,
            string dateColumnName,
            string valueColumnName
        )
        {
            var result = new List<SolarDataDto>();

            using var reader = new StreamReader(fileStream, Encoding.UTF8, true, 1024, true);

            var headerLine = await reader.ReadLineAsync();

            if (headerLine == null)
            {
                return result;
            }

            var headers = ParseCsvLine(headerLine);
            var dateIdx = Array.FindIndex(headers, h => string.Equals(h.Trim(), dateColumnName, StringComparison.OrdinalIgnoreCase));
            var valueIdx = Array.FindIndex(headers, h => string.Equals(h.Trim(), valueColumnName, StringComparison.OrdinalIgnoreCase));

            if (dateIdx < 0 || valueIdx < 0)
            {
                return result;
            }

            while (await reader.ReadLineAsync() is { } line)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                var fields = ParseCsvLine(line);
                if (fields.Length <= Math.Max(dateIdx, valueIdx))
                {
                    continue;
                }

                if (!DateTime.TryParse(fields[dateIdx], out var date))
                {
                    continue;
                }

                if (!decimal.TryParse(fields[valueIdx], NumberStyles.Any, CultureInfo.InvariantCulture, out var value))
                {
                    continue;
                }

                result.Add(new SolarDataDto(date, value));
            }
            return result;
        }

        private static string[] ParseCsvLine(string line)
        {
            // Very simple CSV parsing: split on comma, trim quotes
            var parts = line.Split(",");
            for (var i = 0; i < parts.Length; i++)
            {
                var p = parts[i].Trim();

                if (p.StartsWith($"\"") && p.EndsWith($"\""))
                {
                    p = p.Substring(1, p.Length - 2);
                }

                parts[i] = p;
            }
            return parts;
        }
    }
}
