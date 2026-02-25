using ElectricDashboardApi.Models.Lookups;
using ElectricCompanyEntity = ElectricDashboardApi.Infrastructure.Entities.ElectricCompany;

namespace ElectricDashboardApi.Mappers;

public static class ElectricComapnyMapper
{
    public static ElectricCompany ToModel(ElectricCompanyEntity entity)
    {
        return new ElectricCompany()
        {
            CompanyName = entity.CompanyName,
            ElectricCompanyId = entity.ElectricCompanyId,
            State = entity.State
        };
    }
}
