namespace ElectricDashboardApi.Dtos.OAuth;

public class AdminTokenResult
{
    public bool IsSuccessful { get; init; }
    public string? ErrorMessage { get; init; }
    public string? AdminToken { get; init; }
}
