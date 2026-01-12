using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace ElectricDashboard.Models.User;

public record UserUpdate
{
    [MaxLength(50)]
    public required string FirstName { get; init; }
    
    [MaxLength(50)]
    public required string LastName { get; init; }
    
    public required DateOnly DateOfBirth { get; init; }
}