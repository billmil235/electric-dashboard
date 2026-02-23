using ElectricDashboardApi.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace ElectricDashboardApi.Infrastructure.Queries.Lookups;

public class GetElectricCompanies(ElectricDashboardContext context) : IGetElectricCompanies
{
    public async Task<IReadOnlyList<ElectricCompany>> Execute()
    {
        return await context.ElectricCompanies.ToListAsync();
    }
}
