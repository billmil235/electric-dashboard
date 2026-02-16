namespace ElectricDashboardApi.Dtos.OAuth;

public class RefreshTokenResponse
{
    public string AccessToken { get; set; } = string.Empty;

    public int ExpiresIn { get; set; }

    public string RefreshToken { get; set; } = string.Empty;

    public int RefreshExpiresIn { get; set; }

    public string TokenType { get; set; } = string.Empty;

    public string Scope { get; set; } = string.Empty;
}
