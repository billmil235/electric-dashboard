namespace ElectricDashboardApi.Dtos.DataSources;

public record SolarDataDto(
    DateTime Date,
    decimal Value);
