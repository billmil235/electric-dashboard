using ElectricDashboard.Services.DataSources;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ElectricDashboardApi.Endpoints;

public static class DataSourceEndpoints
{
    public static RouteGroupBuilder RegisterDataEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/electric-bill", () => Results.Ok()).RequireAuthorization();

        group.MapPost("/electric-bill", () => Results.Ok()).RequireAuthorization();

        group.MapPost("/electric-bill/upload",
            async (IFormFile file, IDataSourceService dataSourceService) =>
            {
                using var memoryStream = new MemoryStream();
                await file.CopyToAsync(memoryStream);
                await dataSourceService.ParseUploadedBill(memoryStream, file.ContentType);
                
                return Results.Ok();
            }).RequireAuthorization();
        
        return group;
    }
}