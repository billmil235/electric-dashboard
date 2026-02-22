namespace ElectricDashboardApi.Dtos.OAuth;

public class CreateUserResult
{
    public bool IsSuccessful { get; init; }
    public string? ErrorMessage { get; init; }
    public Guid? UserGuid { get; init; }
}
