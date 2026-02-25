namespace ElectricDashboardApi.Models.Lookups;

public record ElectricCompany
{
    public int ElectricCompanyId { get; init; }

    public string CompanyName { get; init; }

    public string State { get; init; }
}
