using System.ComponentModel.DataAnnotations;

namespace ElectricDashboardApi.Models.User;

public record UserUpdate
{
    [MaxLength(50)]
    public required string FirstName { get; init; }

    [MaxLength(50)]
    public required string LastName { get; init; }

    public required DateOnly DateOfBirth { get; init; }
}
