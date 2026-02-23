using ElectricDashboardApi.Infrastructure.Queries.Lookups;
using ElectricDashboardApi.Mappers;

namespace ElectricDashboardApi.Endpoints;

public static class LookupEndpoints
{
    public static RouteGroupBuilder RegisterLookupEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/electric-companies", async (IGetElectricCompanies getElectricCompanies) =>
        {
            var electricCompanies = await getElectricCompanies.Execute();
            return Results.Ok(electricCompanies.Select(ElectricComapnyMapper.ToModel));
        });

        return group;
    }
}
