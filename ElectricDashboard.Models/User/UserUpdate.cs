namespace ElectricDashboard.Models.User;

public class UserUpdate
{
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required DateOnly DateOfBirth { get; init; }
}