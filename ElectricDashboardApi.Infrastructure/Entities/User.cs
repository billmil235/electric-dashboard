using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElectricDashboardApi.Infrastructure.Entities;

[Table(nameof(User))]
public class User
{
    [Key]
    public required Guid UserId { get; set; }
    
    [MaxLength(50)]
    public required string FirstName { get; set; }
    
    [MaxLength(50)]
    public required string LastName { get; set; }
    
    [MaxLength(100)]
    public required string EmailAddress { get; set; }
    
    public required DateOnly DateOfBirth { get; set; }

    public virtual ICollection<ServiceAddress> ServiceAddresses { get; set; } = new List<ServiceAddress>();
}