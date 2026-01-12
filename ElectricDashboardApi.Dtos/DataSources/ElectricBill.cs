namespace ElectricDashboardApi.Dtos.DataSources;

public record ElectricBill
{
    public Guid? BillId { get; set; }
    
    public Guid? AddressId { get; set; }
    
    public DateOnly PeriodStartDate { get; set; }
    
    public DateOnly PeriodEndDate { get; set; }
    
    public int ConsumptionKwh { get; set; }

    public int SentBackKwh { get; set; } = 0;

    public decimal BilledAmount { get; set; }
}