using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElectricDashboardApi.Data.Entities;

[Table("users")]
public class User
{
    [Key]
    [Column("userid")]
    public required Guid UserId { get; set; }
    
    [MaxLength(50)]
    [Column("firstname")]
    public required string FirstName { get; set; }
    
    [MaxLength(50)]
    [Column("lastname")]
    public required string LastName { get; set; }
    
    [MaxLength(100)]
    [Column("emailaddress")]
    public required string EmailAddress { get; set; }
    
    [Column("dateofbirth")]
    public required DateOnly DateOfBirth { get; set; }
}