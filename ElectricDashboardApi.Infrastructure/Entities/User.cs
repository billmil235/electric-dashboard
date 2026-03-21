using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElectricDashboardApi.Infrastructure.Entities;

[Table("Users")]
public class User
{
    [Key]
    public required Guid UserId { get; set; }

    [Required]
    [MaxLength(50)]
    public required string FirstName { get; set; }

    [Required]
    [MaxLength(50)]
    public required string LastName { get; set; }

    [Required]
    [MaxLength(100)]
    public required string EmailAddress { get; set; }

    [Required]
    public required DateOnly DateOfBirth { get; set; }

    public virtual ICollection<ServiceAddress> ServiceAddresses { get; set; } = new List<ServiceAddress>();
}
