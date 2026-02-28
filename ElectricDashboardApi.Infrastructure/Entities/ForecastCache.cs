using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElectricDashboardApi.Infrastructure.Entities;

[Table(nameof(ForecastCache))]
public class ForecastCache
{
    [Key] public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid AddressId { get; set; }

    [Required]
    public int ForecastYear { get; set; }

    [Required]
    public int ForecastMonth { get; set; }

    [Required]
    public decimal PredictedAmount { get; set; }

    [Required]
    public string AlgorithmUsed { get; set; } = string.Empty;

    [Required]
    public DateTime CachedAt { get; set; } = DateTime.UtcNow;
}
