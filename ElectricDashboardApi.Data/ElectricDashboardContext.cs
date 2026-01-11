using ElectricDashboardApi.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ElectricDashboardApi.Data;

public class ElectricDashboardContext(DbContextOptions<ElectricDashboardContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();

    public DbSet<ElectricBill> ElectricBills => Set<ElectricBill>();

    public DbSet<ServiceAddress> ServiceAddresses => Set<ServiceAddress>();
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.HasDefaultSchema("public");
        base.OnModelCreating(builder);
    }
}