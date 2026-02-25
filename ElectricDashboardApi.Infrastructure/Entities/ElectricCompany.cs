using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElectricDashboardApi.Infrastructure.Entities;

[Table(nameof(ElectricCompany))]
public class ElectricCompany
{
    public int ElectricCompanyId { get; set; }

    public string CompanyName { get; set; }

    [Column("state")]
    [MaxLength(2)]
    public string? State { get; set; }
}
