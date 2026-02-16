using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElectricDashboardApi.Infrastructure.Entities;

[Table(nameof(ElectricBill))]
public class ElectricBill
{
    [Key]
    public Guid BillId { get; set; } = Guid.NewGuid();

    [Required]
    public Guid AddressId { get; set; }

    [ForeignKey(nameof(AddressId))]
    public virtual ServiceAddress ServiceAddress { get; set; }

    [Required]
    public DateTime PeriodStartDate { get; set; }

    [Required]
    public DateTime PeriodEndDate { get; set; }

    [Required]
    public int ConsumptionKwh { get; set; }

    public int? SentBackKwh { get; set; }

    [Required]
    [Column(TypeName = "decimal(10, 2)")]
    public decimal BilledAmount { get; set; }

    [Column(TypeName = "decimal(6,4")]
    public decimal? UnitPrice { get; set; }
}
