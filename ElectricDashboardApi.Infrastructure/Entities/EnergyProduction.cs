using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElectricDashboardApi.Infrastructure.Entities;

[Table(nameof(EnergyProduction))]
public class EnergyProduction
{
    [Key]
    public Guid ProductionId { get; set; }

    public Guid AddressId { get; set; }

    [ForeignKey(nameof(AddressId))]
    public virtual ServiceAddress ServiceAddress { get; set; }

    public DateTime ProductionDate { get; set; }

    public decimal ProductionAmount { get; set; }

    public DateTime CreatedAt { get; set; }
}
