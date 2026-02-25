using ElectricDashboardApi.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace ElectricDashboardApi.Infrastructure.Queries.Lookups;

public class GetElectricCompanies(ElectricDashboardContext context) : IGetElectricCompanies
{
public async Task<IReadOnlyList<ElectricCompany>> Execute(string? countryName, string? stateName)
{
    var query = context.ElectricCompanies.AsQueryable();

    if (!string.IsNullOrEmpty(stateName))
    {
        query = query.Where(ec => ec.State == stateName || ec.State == null);
    }

    return await query.ToListAsync();
}
}
