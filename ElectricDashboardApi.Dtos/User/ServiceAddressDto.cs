namespace ElectricDashboardApi.Dtos.User;

using System.ComponentModel.DataAnnotations;

public record ServiceAddressDto
{
    public Guid AddressId { get; init; }

    [Required]
    [StringLength(100, MinimumLength = 1)]
    public required string AddressName { get; init; }

    [Required]
    [StringLength(255, MinimumLength = 1)]
    public required string AddressLine1 { get; init; }

    [StringLength(255)]
    public string? AddressLine2 { get; init; } = null;

    [Required]
    [StringLength(100, MinimumLength = 1)]
    public required string City { get; init; }

    [Required]
    [StringLength(100, MinimumLength = 1)]
    public required string State { get; init; }

    [Required]
    [RegularExpression(@"^\d{5}(-\d{4})?$")]
    public required string ZipCode { get; init; }

    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string? Country { get; init; } = "USA";

    public bool IsCommercial { get; init; } = false;

    public int ElectricCompanyId { get; init; } = 0;
}
