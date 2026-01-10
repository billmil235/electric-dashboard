using Microsoft.EntityFrameworkCore;

namespace ElectricDashboardApi.Data;

public class ElectricDashboardContext(DbContextOptions<ElectricDashboardContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.HasDefaultSchema("public");
        base.OnModelCreating(builder);
    }
}