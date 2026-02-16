using ElectricDashboardApi.Dtos.DataSources;

namespace ElectricDashboard.Services.DataSources;

public interface IDataSourceService
{
    Task<ElectricBillDto?> ParseUploadedBill(Guid addressId, MemoryStream file, string contentType);
}