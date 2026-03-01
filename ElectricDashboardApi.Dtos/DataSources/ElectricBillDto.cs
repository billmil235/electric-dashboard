namespace ElectricDashboardApi.Dtos.DataSources;

public record ElectricBillDto
{
    public Guid? AddressId { get; init; }

    public DateOnly PeriodStartDate { get; init; }

    public DateOnly PeriodEndDate { get; init; }

    public int ConsumptionKwh { get; init; }

    public int? SentBackKwh { get; init; }

    public decimal BilledAmount { get; init; }

    public decimal? UnitPrice { get; init; }

    public List<ElectricBillLineItemDto>? LineItemCharges { get; init; }
}
