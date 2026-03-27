using ElectricDashboard.Services.DataSources;
using ElectricDashboard.Services.User;
using ElectricDashboardApi.Infrastructure.Commands.DataSources;
using ElectricDashboardApi.Infrastructure.Commands.Forecast;
using ElectricDashboardApi.Infrastructure.Commands.User;
using ElectricDashboardApi.Infrastructure.Queries.DataSources;
using ElectricDashboardApi.Infrastructure.Queries.Forecast;
using ElectricDashboardApi.Infrastructure.Queries.Lookups;
using ElectricDashboardApi.Infrastructure.Queries.User;
using ElectricDashboardApi.Infrastructure.Services.Forecast;
using ElectricDashboardApi.Infrastructure.Services.Solar;
using ElectricDashboardApi.Models.Options;
using OllamaSharp;

namespace ElectricDashboardApi;

public static class DependencyInjection
{
    public static void RegisterServices(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<KeycloakOptions>(
            builder.Configuration.GetSection("Keycloak")
        );

        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<IDataSourceService, DataSourceService>();
        builder.Services.AddSingleton<ISolarDataService, SolarDataService>();

        builder.Services.AddScoped<IUpdateProfileCommand, UpdateProfileCommand>();

        builder.Services.AddScoped<IAddElectricBillCommand, AddElectricBillCommand>();

        builder.Services.AddScoped<IGetElectricBillQuery, GetElectricBillsQuery>();

        builder.Services.AddScoped<IUserAddressService, UserAddressService>();

        builder.Services.AddScoped<IGetAddressExistsQuery, GetAddressExistsQuery>();
        builder.Services.AddScoped<IGetUserAddressesQuery, GetUserAddressesQuery>();
        builder.Services.AddScoped<IAddServiceAddressCommand, AddServiceAddressCommand>();
        builder.Services.AddScoped<IUpdateServiceAddressCommand, UpdateServiceAddressCommand>();
        builder.Services.AddScoped<IDeleteServiceAddressCommand, DeleteServiceAddressCommand>();
        builder.Services.AddScoped<IDeleteElectricBillCommand, DeleteElectricBillCommand>();

        builder.Services.AddScoped<IGetElectricCompanies, GetElectricCompanies>();

        builder.Services.AddScoped<IForecastService, ForecastService>();
        builder.Services.AddScoped<ICacheForecastCommand, CacheForecastCommand>();
        builder.Services.AddScoped<IGetForecastQuery, GetForecastQuery>();

        builder.Services.AddHttpClient<IOllamaApiClient, OllamaApiClient>("ollama", client =>
        {
            client.BaseAddress = new Uri("http://192.168.1.135:11434/");
            client.Timeout = TimeSpan.FromMinutes(15);
        });

        builder.Services.AddChatClient(sp =>
        {
            var httpClient = sp.GetRequiredService<IHttpClientFactory>()
                .CreateClient("ollama");

            return new OllamaApiClient(httpClient, "devstral-small-2:24b");
            //return new OllamaApiClient(httpClient, "qwen3.5:27b");
        });
    }
}
