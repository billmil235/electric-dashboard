namespace ElectricDashboard.Models.Options;

public class KeycloakOptions
{
    public required string TokenUrl { get; init; }
    public required string UserUrl { get; init; }
    public required string ClientSecret { get; init; }
    public required string ClientId { get; init; }
    public required string MetadataAddress { get; init; }
    public required string ValidIssuer { get; init; }
    public required string AuthorizationUrl { get; init; }
}