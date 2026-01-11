namespace ElectricDashboard.Models.Keycloak;

public class TokenResponse
{
    public string Access_token { get; set; } = string.Empty;
    
    public int Expires_in { get; set; }
    
    public string Refresh_token { get; set; } = string.Empty;
    
    public int Refresh_expires_in { get; set; }
    
    public string Token_type { get; set; } = string.Empty;
    
    public string Scope { get; set; } = string.Empty;
}