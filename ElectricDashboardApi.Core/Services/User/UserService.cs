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
    IUpdateProfileCommand updateProfileCommand,
    HttpClient client) : IUserService
{

    private readonly JsonSerializerOptions _options = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
    };

    private async Task<string> GetAdminTokenAsync()
    {
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
            var error = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            throw new Exception($"Keycloak error: {response.StatusCode} - {error}");
        }

        var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        using var document = JsonDocument.Parse(json);
        return document.RootElement.GetProperty("access_token").GetString()!;
    }

    public async Task<bool> ExistsByEmailAsync(string emailAddress)
    {
        var token = await GetAdminTokenAsync().ConfigureAwait(false);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var requestUri = $"{options.Value.UserUrl}?email={Uri.EscapeDataString(emailAddress)}&max=1";
        var response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Get, requestUri)).ConfigureAwait(false);

        var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Keycloak error: {response.StatusCode} - {content}");
        }

        using var usersDoc = JsonDocument.Parse(content);
        var users = usersDoc.RootElement.EnumerateArray();
        return users.Any(user =>
        {
            string? userEmail = user.TryGetProperty("email", out var emailProperty)
                ? emailProperty.GetString()
                : null;

            if (userEmail is not null && string.Equals(userEmail, emailAddress, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return user.TryGetProperty("username", out var usernameProperty)
                && string.Equals(usernameProperty.GetString(), emailAddress, StringComparison.OrdinalIgnoreCase);
        });
    }

    public async Task<CreateUserResult> CreateUserAsync(UserDto userModel)
    {
        var token = await GetAdminTokenAsync().ConfigureAwait(false);

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        if (await ExistsByEmailAsync(userModel.EmailAddress).ConfigureAwait(false))
        {
            return new CreateUserResult()
            {
                IsSuccessful = false,
                EmailAlreadyExists = true,
                ErrorMessage = "Email already exists"
            };
        }

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
            var error = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
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
                    UserGuid = userId
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
        var content = new FormUrlEncodedContent([
            new KeyValuePair<string, string>("client_id", options.Value.ClientId),
            new KeyValuePair<string, string>("client_secret", options.Value.ClientSecret),
            new KeyValuePair<string, string>("grant_type", "password"),
            new KeyValuePair<string, string>("username", username),
            new KeyValuePair<string, string>("password", password)
        ]);

        var response = await client.PostAsync(
            options.Value.TokenUrl,
            content).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var errorResponse = JsonSerializer.Deserialize<KeyCloakLoginError>(error);

            return new LoginResult()
            {
                IsSuccessful = false,
                ErrorMessage = errorResponse?.ErrorDescription
            };
        }

        var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        using var document = JsonDocument.Parse(json);

        document.RootElement.TryGetProperty("access_token", out var accessTokenProp);
        var accessToken = accessTokenProp.GetString();

        return new LoginResult()
        {
            IsSuccessful = accessToken != null,
            Token = new LoginTokenResponse
            {
                AccessToken = accessToken ?? string.Empty,
                TokenType = document.RootElement.TryGetProperty("token_type", out var tt) ? tt.GetString()! : string.Empty,
                ExpiresIn = document.RootElement.TryGetProperty("expires_in", out var ei) ? ei.GetInt32() : 0,
                RefreshExpiresIn = document.RootElement.TryGetProperty("refresh_expires_in", out var rei) ? rei.GetInt32() : 0,
                RefreshToken = document.RootElement.TryGetProperty("refresh_token", out var rt) ? rt.GetString()! : string.Empty,
                SessionState = document.RootElement.TryGetProperty("session_state", out var ss) ? ss.GetString()! : string.Empty,
                Scope = document.RootElement.TryGetProperty("scope", out var s) ? s.GetString()! : string.Empty,
                NotBeforePolicy = document.RootElement.TryGetProperty("not_before_policy", out var np) ? np.GetInt32() : 0
            }
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
        ).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            throw new Exception($"Error refreshing Keycloak token: {response.StatusCode}, {error}");
        }

        var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        using var document = JsonDocument.Parse(json);
        var tokenResponse = document.RootElement.GetProperty("access_token").GetString();

        return new RefreshTokenResponse
        {
            AccessToken = tokenResponse!,
            ExpiresIn = document.RootElement.GetProperty("expires_in").GetInt32(),
            RefreshToken = document.RootElement.TryGetProperty("refresh_token", out var rt) ? rt.GetString()! : string.Empty,
            RefreshExpiresIn = document.RootElement.TryGetProperty("refresh_expires_in", out var rei) ? rei.GetInt32() : 0,
            TokenType = document.RootElement.TryGetProperty("token_type", out var tt) ? tt.GetString()! : string.Empty,
            Scope = document.RootElement.TryGetProperty("scope", out var s) ? s.GetString()! : string.Empty
        };
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
