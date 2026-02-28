namespace ElectricDashboardApi.Dtos.Forecast;

public record ForecastResponse
{
    public Guid AddressId {get; set;}

    public int ForecastYear {get; set;}

    public int ForecastMonth {get; set;}

    public decimal PredictedAmount {get; set;}

    public string AlgorithmUsed { get; set; }

    public decimal Confidence { get; set; }
}
