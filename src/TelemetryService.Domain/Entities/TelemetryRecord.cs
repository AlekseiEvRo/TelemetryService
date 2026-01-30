namespace TelemetryService.Domain.Entities;

public class TelemetryRecord
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid DeviceId { get; set; }
    public Device Device { get; set; } = default!;

    public DateTimeOffset Timestamp { get; set; }

    public decimal? Temperature { get; set; }
    public int? BatteryLevel { get; set; } // 0..100
    public int? SignalRssi { get; set; }   // e.g. -120..0
    public bool? IsOnline { get; set; }
}