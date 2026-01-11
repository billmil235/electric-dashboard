using System.ComponentModel.DataAnnotations;

namespace ElectricDashboard.Models.User;

public record User
{
    [MaxLength(50)]
    public required string FirstName { get; init; }
    
    [MaxLength(50)]
    public required string LastName { get; init; }
    
    [MaxLength(100)]
    public required string EmailAddress { get; init; }
    
    public required string Password { get; init; }
    
    public required DateOnly DateOfBirth { get; init; }
}