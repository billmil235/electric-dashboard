using ServiceAddressModel = ElectricDashboard.Models.User.ServiceAddress;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElectricDashboardApi.Data.Entities;

[Table(nameof(ServiceAddress))]
public class ServiceAddress
{
    [Key]
    public Guid AddressId { get; set; } = Guid.NewGuid();
    
    public Guid UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public virtual User User { get; set; }

    [StringLength(100)]
    public required string AddressName { get; set; }

    [StringLength(100)]
    public required string AddressLine1 { get; set; }

    [StringLength(100)]
    public string? AddressLine2 { get; set; }

    [StringLength(50)]
    public required string City { get; set; }

    [StringLength(50)]
    [Column("state")]
    public required string State { get; set; }

    [StringLength(10)]
    public required string ZipCode { get; set; }

    [StringLength(50)]
    public string? Country { get; set; }
    
    public bool IsCommercial { get; set; }

    public ServiceAddressModel ToModel()
    {
        return new ServiceAddressModel
        {
            AddressId = AddressId,
            AddressName = AddressName,
            AddressLine1 = AddressLine1,
            AddressLine2 = AddressLine2,
            City = City,
            State = State,
            ZipCode = ZipCode,
            Country = Country,
            IsCommercial = IsCommercial
        };
    }
}