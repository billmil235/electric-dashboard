namespace ElectricDashboardApi.Dtos.OAuth;

public class LoginResult
{
    public bool IsSuccessful { get; init; }
    public string? ErrorMessage { get; init; }

    public LoginTokenResponse? Token { get; init; }
}
