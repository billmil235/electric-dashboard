using Microsoft.EntityFrameworkCore;
using ElectricDashboardApi.Infrastructure.Entities;
using ElectricDashboardApi.Dtos.DataSources;

namespace ElectricDashboardApi.Infrastructure.Commands.DataSources;

public class AddEnergyProductionCommand(ElectricDashboardContext context) : IAddEnergyProductionCommand
{
    public async Task<EnergyProduction?> AddEnergyProduction(Guid userId, Guid addressId, SolarDataDto dto, Guid? productionId = null)
    {
        EnergyProduction? entity;
        if (productionId.HasValue)
        {
            entity = await context.EnergyProductions
                .FirstOrDefaultAsync(x => x.ProductionId == productionId.Value)
                .ConfigureAwait(false) ?? new EnergyProduction();
        }
        else
        {
            entity = new EnergyProduction
            {
                ProductionId = productionId ?? Guid.CreateVersion7(),
                AddressId = addressId,
                ProductionDate = dto.Date,
                ProductionAmount = dto.Value
            };
        }

        await context.EnergyProductions.AddAsync(entity).ConfigureAwait(false);
        await context.SaveChangesAsync().ConfigureAwait(false);
        return entity;
    }
}
