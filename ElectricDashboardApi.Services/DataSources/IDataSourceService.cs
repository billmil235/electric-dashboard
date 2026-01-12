using ElectricDashboardApi.Data.Entities;

namespace ElectricDashboard.Services.DataSources;

public interface IDataSourceService
{
    Task<ElectricBill?> ParseUploadedBill(MemoryStream file, string contentType);
}