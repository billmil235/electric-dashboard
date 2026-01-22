using ElectricDashboardApi.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ElectricDashboardApi.Data;

public class ElectricDashboardContext(DbContextOptions<ElectricDashboardContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();

    public DbSet<ServiceProvider> ServiceProviders => Set<ServiceProvider>();

    public DbSet<ElectricBill> ElectricBills => Set<ElectricBill>();

    public DbSet<UserToServiceAddress> UserToServiceAddresses { get; set; }
    
    public DbSet<ServiceAddress> ServiceAddresses => Set<ServiceAddress>();
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.HasDefaultSchema("public");
        
        builder.Entity<UserToServiceAddress>()
            .HasKey(u => new { u.UserId, u.AddressId });

        builder.Entity<UserToServiceAddress>()
            .HasOne(u => u.User)
            .WithMany()
            .HasForeignKey(u => u.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<UserToServiceAddress>()
            .HasOne(u => u.ServiceAddress)
            .WithMany()
            .HasForeignKey(u => u.AddressId)
            .OnDelete(DeleteBehavior.Restrict);
        
        base.OnModelCreating(builder);
    }
}