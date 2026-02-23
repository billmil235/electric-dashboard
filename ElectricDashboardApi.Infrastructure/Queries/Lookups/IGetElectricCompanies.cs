using ElectricDashboardApi.Infrastructure.Entities;

namespace ElectricDashboardApi.Infrastructure.Queries.Lookups;

public interface IGetElectricCompanies
{
    Task<IReadOnlyList<ElectricCompany>> Execute();
}
