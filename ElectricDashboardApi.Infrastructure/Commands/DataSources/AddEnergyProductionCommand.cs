namespace ElectricDashboardApi.Infrastructure.Commands.DataSources;

using ElectricDashboardApi.Dtos.DataSources;
using Entities;
using Microsoft.EntityFrameworkCore;

public class AddEnergyProductionCommand(ElectricDashboardContext context) : IAddEnergyProductionCommand
{
    public async Task<EnergyProduction?> AddEnergyProduction(Guid userId, Guid addressId, SolarDataDto dto, Guid? productionId = null)
    {
        if (productionId.HasValue)
        {
            var entity = await context.EnergyProductions
                .FirstOrDefaultAsync(x => x.ProductionId == productionId.Value)
                .ConfigureAwait(false);

            if (entity == null)
            {
                await context.EnergyProductions.AddAsync(new EnergyProduction
                {
                    ProductionId = productionId.Value,
                    AddressId = addressId,
                    ProductionDate = dto.Date,
                    ProductionAmount = dto.Value,
                    CreatedAt = DateTime.UtcNow
                }).ConfigureAwait(false);
            }
            else
            {
                entity.ProductionDate = dto.Date;
                entity.ProductionAmount = dto.Value;
            }

            await context.SaveChangesAsync().ConfigureAwait(false);
            return entity;
        }
        else
        {
            var entity = new EnergyProduction
            {
                ProductionId = Guid.CreateVersion7(),
                AddressId = addressId,
                ProductionDate = dto.Date,
                ProductionAmount = dto.Value,
                CreatedAt = DateTime.UtcNow
            };

            await context.EnergyProductions.AddAsync(entity).ConfigureAwait(false);
            await context.SaveChangesAsync().ConfigureAwait(false);
            return entity;
        }
    }
}
