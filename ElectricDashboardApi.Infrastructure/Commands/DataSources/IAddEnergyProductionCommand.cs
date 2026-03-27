using ElectricDashboardApi.Dtos.DataSources;
using ElectricDashboardApi.Infrastructure.Entities;

namespace ElectricDashboardApi.Infrastructure.Commands.DataSources;

public interface IAddEnergyProductionCommand
{
    Task<EnergyProduction?> AddEnergyProduction(Guid userId, Guid addressId, SolarDataDto dto, Guid? productionId = null);
}
