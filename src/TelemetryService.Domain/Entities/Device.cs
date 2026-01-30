using TelemetryService.Domain.Enums;

namespace TelemetryService.Domain.Entities;

public class Device
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string ExternalId { get; set; } = default!; // unique
    public string Name { get; set; } = default!;
    public DeviceType Type { get; set; } = DeviceType.Sensor;
    public DeviceStatus Status { get; set; } = DeviceStatus.Active;

    public string? Location { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public List<TelemetryRecord> TelemetryRecords { get; set; } = new();
}