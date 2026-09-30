using System.Net.Http;
using System.Text.Json;

using ElectricDashboardApi.Models.Options;

using Microsoft.Extensions.Options;

namespace ElectricDashboard.Services.User;

public class KeycloakTokenClient : IKeycloakTokenClient
{
    private readonly HttpClient _client;
    private readonly KeycloakOptions _options;

    public KeycloakTokenClient(HttpClient client, IOptions<KeycloakOptions> options)
    {
        _client = client;
        _options = options.Value;
    }

    public async Task<KeycloakTokenResponse?> GetTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        var requestData = new Dictionary<string, string>
        {
            { "grant_type", "refresh_token" },
            { "client_id", _options.ClientId },
            { "client_secret", _options.ClientSecret },
            { "refresh_token", refreshToken }
        };

        var requestContent = new FormUrlEncodedContent(requestData);

        var request = new HttpRequestMessage(HttpMethod.Post, _options.TokenUrl) {
            Content = requestContent
        };

        var response = await _client.SendAsync(request, cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            throw new Exception($"Error refreshing Keycloak token: {response.StatusCode}");
        }

        var json = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        using var document = JsonDocument.Parse(json);
        var tokenResponse = document.RootElement.GetProperty("access_token").GetString();

        return new KeycloakTokenResponse
        {
            AccessToken = tokenResponse!,
            ExpiresIn = document.RootElement.GetProperty("expires_in").GetInt32(),
            RefreshToken = document.RootElement.TryGetProperty("refresh_token", out var rt) ? rt.GetString()! : string.Empty,
            RefreshExpiresIn = document.RootElement.TryGetProperty("refresh_expires_in", out var rei) ? rei.GetInt32() : 0,
            TokenType = document.RootElement.TryGetProperty("token_type", out var tt) ? tt.GetString()! : string.Empty,
            Scope = document.RootElement.TryGetProperty("scope", out var s) ? s.GetString()! : string.Empty
        };
    }
}

public interface IKeycloakTokenClient
{
    Task<KeycloakTokenResponse?> GetTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
}

public class KeycloakTokenResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public int ExpiresIn { get; set; }
    public string RefreshToken { get; set; } = string.Empty;
    public int RefreshExpiresIn { get; set; }
    public string TokenType { get; set; } = string.Empty;
    public string Scope { get; set; } = string.Empty;
}