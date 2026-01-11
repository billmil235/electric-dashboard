using System.Runtime.InteropServices.Marshalling;

namespace ElectricDashboardApi.Data.Entities;

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("electricbills")]
public class ElectricBill
{
    [Key] 
    [Column("billid")]
    public Guid BillId { get; set; } = Guid.NewGuid();

    [Required]
    [Column("addressid")]
    public Guid AddressId { get; set; }

    [ForeignKey(nameof(AddressId))]
    public virtual ServiceAddress ServiceAddress { get; set; }

    [Required]
    [Column("pariodstartdate")]
    public DateTime PeriodStartDate { get; set; }

    [Required]
    [Column("pariodenddate")]
    public DateTime PeriodEndDate { get; set; }

    [Required]
    [Column("consumptionkwh", TypeName = "decimal(10, 2)")]
    public decimal ConsumptionKwh { get; set; }

    [Column("sendbackkwh", TypeName = "decimal(10, 2)")]
    public decimal? SentBackKwh { get; set; }

    [Required]
    [Column("billedamount", TypeName = "decimal(10, 2)")]
    public decimal BilledAmount { get; set; }
}
