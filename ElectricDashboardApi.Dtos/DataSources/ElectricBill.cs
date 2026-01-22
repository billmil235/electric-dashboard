namespace ElectricDashboardApi.Dtos.DataSources;

public record ElectricBill
{
    public Guid? BillId { get; init; }
    
    public Guid? AddressId { get; init; }
    
    public DateOnly PeriodStartDate { get; init; }
    
    public DateOnly PeriodEndDate { get; init; }
    
    public int ConsumptionKwh { get; init; }

    public int? SentBackKwh { get; init; }

    public decimal BilledAmount { get; init; }
    
    public decimal? UnitPrice { get; init; }
}