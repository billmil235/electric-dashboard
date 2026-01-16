namespace ElectricDashboard.Models.User;

public record ServiceAddress
{
    public Guid AddressId { get; init; }
    
    public required string AddressName { get; init; }
    
    public required string AddressLine1 { get; init; }
    
    public string? AddressLine2 { get; init; }
    
    public required string City { get; init; }
    
    public required string State { get; init; }
    
    public required string ZipCode { get; init; }
    
    public string? Country { get; init; }
}