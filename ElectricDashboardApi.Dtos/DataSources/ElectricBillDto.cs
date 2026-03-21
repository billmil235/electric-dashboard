namespace ElectricDashboardApi.Dtos.DataSources;

public record ElectricBillDto (
    Guid? AddressId,
    DateOnly PeriodStartDate,
    DateOnly PeriodEndDate,
    int ConsumptionKwh,
    int? SentBackKwh,
    decimal BilledAmount,
    decimal? UnitPrice,
    List<ElectricBillLineItemDto>? LineItemCharges
);
