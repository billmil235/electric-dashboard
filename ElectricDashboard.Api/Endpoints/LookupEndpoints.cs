using ElectricDashboardApi.Infrastructure.Queries.Lookups;
using ElectricDashboardApi.Mappers;
using Microsoft.AspNetCore.Mvc;

namespace ElectricDashboardApi.Endpoints;

public static class LookupEndpoints
{
    public static RouteGroupBuilder RegisterLookupEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/electric-companies", async ([FromQuery] string? countryName, [FromQuery] string? stateName, IGetElectricCompanies getElectricCompanies) =>
        {
            var electricCompanies = await getElectricCompanies.Execute(countryName, stateName);
            return Results.Ok(electricCompanies.Select(ElectricComapnyMapper.ToModel));
        });

        return group;
    }
}
