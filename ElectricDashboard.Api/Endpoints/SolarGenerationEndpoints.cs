using Microsoft.AspNetCore.Mvc;
using ElectricDashboardApi.Dtos;
using ElectricDashboardApi.Infrastructure.Services.Solar;
using Microsoft.AspNetCore.Http;
using System.Text;
using System.Threading.Tasks;

namespace ElectricDashboardApi.Endpoints;

public static class SolarGenerationEndpoints
{
    public static RouteGroupBuilder RegisterSolarEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("/header", async (IFormFile file, ISolarDataService service) =>
        {
            await using var stream = file.OpenReadStream();
            var headerDto = await service.GetCsvHeader(stream, file.ContentType);
            return Results.Ok(headerDto);
        })
        .DisableAntiforgery();

        group.MapPost("/data", async (IFormFile file, [FromForm] string dateColumn, [FromForm] string valueColumn, ISolarDataService service) =>
        {
            await using var stream = file.OpenReadStream();
            var data = await service.ParseCsvWithColumns(stream, file.ContentType, dateColumn, valueColumn);
            return Results.Ok(data);
        })
        .DisableAntiforgery();

        return group;
    }
}
