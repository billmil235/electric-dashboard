namespace ElectricDashboardApi.Dtos.OAuth;

public class LoginTokenResponse
{
    public string AccessToken { get; set; } = string.Empty;

    public string TokenType { get; set; } = string.Empty;

    public int ExpiresIn { get; set; }

    public int RefreshExpiresIn { get; set; }

    public string RefreshToken { get; set; } = string.Empty;

    public int NotBeforePolicy { get; set; }

    public string SessionState { get; set; } = string.Empty;

    public string Scope { get; set; } = string.Empty;
}
