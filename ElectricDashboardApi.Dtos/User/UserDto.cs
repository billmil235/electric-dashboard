using System.ComponentModel.DataAnnotations;

namespace ElectricDashboardApi.Dtos.User;

public class UserDto
{
    [MaxLength(50)]
    public required string FirstName { get; init; }

    [MaxLength(50)]
    public required string LastName { get; init; }

    [MaxLength(100)]
    public required string EmailAddress { get; init; }

    public required string Password { get; init; }

    public required DateTime DateOfBirth { get; init; }
}
