using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElectricDashboardApi.Data.Entities;

[Table(nameof(ServiceProvider))]
public class ServiceProvider
{
    [Key]
    public int ServiceProviderId { get; set; }
    
    [MaxLength(50)]
    public string ServiceProviderName { get; set; }
}