namespace ElectricDashboardApi.Dtos.OAuth;

public record CreateUserCredential(string Value)
{
    public string Type { get; init; } = "password";
    public bool Temporary { get; init; } = false;
}
