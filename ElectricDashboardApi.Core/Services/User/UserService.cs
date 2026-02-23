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
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly JsonSerializerOptions _optionSnakeCase = new JsonSerializerOptions
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

    public async Task<CreateUserResult> CreateUserAsync(UserDto userModel)
    {
        var token = await GetAdminTokenAsync();

        var client = new HttpClient();

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var user = new CreateUserRequest(
            userModel.EmailAddress,
            userModel.FirstName,
            userModel.LastName,
            userModel.Password);

        var json = JsonSerializer.Serialize(user, _options);

        var content = new StringContent(
            json,
            Encoding.UTF8,
            "application/json");

        var response = await client.PostAsync(
            options.Value.UserUrl,
            content);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            var keycloakError = JsonSerializer.Deserialize<KeyCloakError>(error);
            return new CreateUserResult()
            {
                IsSuccessful = false,
                ErrorMessage = keycloakError?.Error
            };
        }

        try
        {
            var location = response.Headers.Location?.ToString();
            if (location is not null)
            {
                var userId = new Guid(location.Split('/').Last());
                await UpdateUserProfile(userModel, userId);

                return new CreateUserResult()
                {
                    IsSuccessful = true,
                    UserGuid = new Guid(location)
                };
            }
            else
            {
                throw new Exception("Failed to create user.");
            }
        }
        catch (Exception ex)
        {
            return new CreateUserResult()
            {
                IsSuccessful = false,
                ErrorMessage = ex.Message
            };
        }

    }

    public async Task<LoginResult> LoginAsync(string username, string password)
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
            var errorResponse = JsonSerializer.Deserialize<KeyCloakLoginError>(error, _optionSnakeCase);

            return new LoginResult()
            {
                IsSuccessful = false,
                ErrorMessage = errorResponse?.ErrorDescription
            };
        }

        var json = await response.Content.ReadAsStringAsync();

        var loginTokenResponse = JsonSerializer.Deserialize<LoginTokenResponse>(json, _optionSnakeCase); // contains access_token, refresh_token, etc.

        return new LoginResult()
        {
            IsSuccessful = loginTokenResponse != null,
            Token = loginTokenResponse
        };
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

    public Task<UserDto> GetUserInformation(Guid userGuid)
    {
        throw new NotImplementedException();
    }
}
