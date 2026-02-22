namespace ElectricDashboardApi.Dtos.OAuth;

public record CreateUserRequest
{
    public string Username { get; init; }

    public string Email { get; init; }

    public string FirstName { get; init; }

    public string LastName { get; init; }

    public bool Enabled { get; init; } = true;

    public bool EmailVerified { get; init; } = true;

    public List<CreateUserCredential> Credentials { get; init; } = [];

    public CreateUserRequest(string email, string firstname, string lastname, string password)
    {
        Username = email;
        Email = email;
        FirstName = firstname;
        LastName = lastname;
        Credentials.Add(new CreateUserCredential(password));
    }
}
