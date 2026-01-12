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
        
        builder.Services.AddChatClient(new OllamaApiClient("http://192.168.1.135:11434/", "qwen2.5vl:7b"));
    }
}