using Microsoft.EntityFrameworkCore;
using TelemetryService.Domain.Entities;

namespace TelemetryService.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

    public DbSet<Device> Devices => Set<Device>();
    public DbSet<TelemetryRecord> TelemetryRecords => Set<TelemetryRecord>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Device>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.ExternalId).IsRequired().HasMaxLength(100);
            b.Property(x => x.Name).IsRequired().HasMaxLength(200);
            b.HasIndex(x => x.ExternalId).IsUnique();

            b.HasMany(x => x.TelemetryRecords)
                .WithOne(x => x.Device)
                .HasForeignKey(x => x.DeviceId);
        });

        modelBuilder.Entity<TelemetryRecord>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.Timestamp).IsRequired();

            b.HasIndex(x => new { x.DeviceId, x.Timestamp }).IsUnique();
           
            b.HasCheckConstraint("CK_Telemetry_BatteryLevel_Range",
                "\"BatteryLevel\" IS NULL OR (\"BatteryLevel\" >= 0 AND \"BatteryLevel\" <= 100)");
        });
    }
}