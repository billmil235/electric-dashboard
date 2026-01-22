using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElectricDashboardApi.Data.Entities;

[Table("serviceaddresses")]
public class ServiceAddress
{
    [Key]
    [Column("addressid")]
    public Guid AddressId { get; set; } = Guid.NewGuid();

    [Column("userid")]
    public Guid UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public virtual User User { get; set; }

    [StringLength(100)]
    [Column("addressname")]
    public required string AddressName { get; set; }

    [StringLength(100)]
    [Column("addressline1")]
    public required string AddressLine1 { get; set; }

    [StringLength(100)]
    [Column("addressline2")]
    public string? AddressLine2 { get; set; }

    [StringLength(50)]
    [Column("city")]
    public required string City { get; set; }

    [StringLength(50)]
    [Column("state")]
    public required string State { get; set; }

    [StringLength(10)]
    [Column("zipcode")]
    public required string ZipCode { get; set; }

    [StringLength(50)]
    [Column("country")]
    public string? Country { get; set; }
    
    [Column("iscommercial")]
    public bool IsCommercial { get; set; }
}