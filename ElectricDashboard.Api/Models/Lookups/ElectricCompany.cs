namespace ElectricDashboardApi.Models.Lookups;

public record ElectricCompany
{
    public int ElectricCompanyId { get; init; }

    public required string CompanyName { get; init; }

    public required string State { get; init; }
}
