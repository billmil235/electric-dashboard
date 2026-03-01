namespace ElectricDashboardApi.Dtos.DataSources;

public record ElectricBillLineItemDto(string Description, decimal Quantity, decimal UnitPrice, decimal Cost);
