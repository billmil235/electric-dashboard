namespace ElectricDashboardApi.Dtos.OAuth;

public class CreateUserResult
{
    public bool IsSuccessful { get; init; }
    public bool EmailAlreadyExists { get; init; }
    public string? ErrorMessage { get; init; }
    public Guid? UserGuid { get; init; }
}
