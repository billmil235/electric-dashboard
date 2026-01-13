using ElectricDashboard.Models.Options;
using ElectricDashboard.Services.DataSources;
using ElectricDashboard.Services.User;
using ElectricDashboardApi.Data.Commands.DataSources;
using ElectricDashboardApi.Data.Commands.User;
using OllamaSharp;

namespace ElectricDashboardApi;

public static class DependencyInjection
{
    public static void RegisterServices(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<KeycloakOptions>(
            builder.Configuration.GetSection("Keycloak"));

        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<IDataSourceService, DataSourceService>();
        
        builder.Services.AddScoped<IUpdateProfileCommand, UpdateProfileCommand>();
        builder.Services.AddScoped<IAddElectricBillCommand, AddElectricBillCommand>();
        
        builder.Services.AddHttpClient<IOllamaApiClient, OllamaApiClient>("ollama", client =>
        {
            client.BaseAddress = new Uri("http://192.168.1.135:11434/");
            client.Timeout = TimeSpan.FromMinutes(15);
        });

        builder.Services.AddChatClient(sp =>
        {
            var httpClient = sp.GetRequiredService<IHttpClientFactory>()
                .CreateClient("ollama");

            return new OllamaApiClient(httpClient, "gpt-oss");
        });
    }
}