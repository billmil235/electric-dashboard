namespace ElectricDashboardApi.Dtos.OAuth;

public record KeyCloakLoginError
{
    public string? Error { get; init; }
    public string? ErrorDescription { get; init; }
}
