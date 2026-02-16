using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using ElectricDashboardApi.Dtos.OAuth;
using ElectricDashboardApi.Dtos.User;
using ElectricDashboardApi.Infrastructure.Commands.User;
using ElectricDashboardApi.Models.Options;
using Microsoft.Extensions.Options;

namespace ElectricDashboard.Services.User;

public class UserService(
    IOptions<KeycloakOptions> options,
    IUpdateProfileCommand updateProfileCommand) : IUserService
{

    private readonly JsonSerializerOptions _options = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
    };

    private async Task<string> GetAdminTokenAsync()
    {
        var client = new HttpClient();

        var content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("client_id", options.Value.ClientId),
            new KeyValuePair<string, string>("client_secret", options.Value.ClientSecret),
            new KeyValuePair<string, string>("grant_type", "client_credentials")
        });

        var response = await client.PostAsync(
            options.Value.TokenUrl,
            content);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Keycloak error: {response.StatusCode} - {error}");
        }

        var json = await response.Content.ReadAsStringAsync();
        return JsonDocument.Parse(json).RootElement.GetProperty("access_token").GetString();
    }

    public async Task CreateUserAsync(UserDto userModel)
    {
        var token = await GetAdminTokenAsync();

        var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var user = new
        {
            username = userModel.EmailAddress,
            email = userModel.EmailAddress,
            enabled = true,
            emailVerified = true,
            firstName = userModel.FirstName,
            lastName = userModel.LastName,
            credentials = new[]
            {
                new
                {
                    type = "password",
                    value = userModel.Password,
                    temporary = false
                }
            }
        };

        var content = new StringContent(
            JsonSerializer.Serialize(user),
            Encoding.UTF8,
            "application/json");

        var response = await client.PostAsync(
            options.Value.UserUrl,
            content);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Keycloak error: {response.StatusCode} - {error}");
        }

        var location = response.Headers.Location?.ToString();
        if (location is not null)
        {
            var userId = new Guid(location.Split('/').Last());
            await UpdateUserProfile(userModel, userId);
        }
    }

    public async Task<LoginTokenResponse> LoginAsync(string username, string password)
    {
        var client = new HttpClient();

        var content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("client_id", options.Value.ClientId),
            new KeyValuePair<string, string>("client_secret", options.Value.ClientSecret),
            new KeyValuePair<string, string>("grant_type", "password"),
            new KeyValuePair<string, string>("username", username),
            new KeyValuePair<string, string>("password", password)
        });

        var response = await client.PostAsync(
            options.Value.TokenUrl,
            content);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Keycloak error: {response.StatusCode} - {error}");
        }

        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<LoginTokenResponse>(json, _options) ?? new LoginTokenResponse(); // contains access_token, refresh_token, etc.
    }

    public async Task<RefreshTokenResponse?> RefreshTokenAsync(string refreshToken)
    {
        var requestData = new Dictionary<string, string>
        {
            { "grant_type", "refresh_token" },
            { "client_id", options.Value.ClientId },
            { "client_secret", options.Value.ClientSecret },
            { "refresh_token", refreshToken }
        };

        var requestContent = new FormUrlEncodedContent(requestData);

        var client = new HttpClient();

        var response = await client.PostAsync(
            options.Value.TokenUrl,
            requestContent
        );

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Error refreshing Keycloak token: {response.StatusCode}, {error}");
        }

        var json = await response.Content.ReadAsStringAsync();
        var tokenResponse = JsonSerializer.Deserialize<RefreshTokenResponse>(json, _options);

        return tokenResponse;
    }

    public async Task UpdateUserProfile(UserDto user, Guid userId)
    {
        await updateProfileCommand.Execute(user, userId);
    }
}
