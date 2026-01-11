namespace ElectricDashboard.Models.User;

public class User
{
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string UserName { get; init; }
    public required string EmailAddress { get; init; }
    public required string Password { get; init; }
}