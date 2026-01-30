using TelemetryService.Domain.Enums;

namespace TelemetryService.WebApi.Contracts.Devices;

public class DeviceResponse
{
    public Guid Id { get; set; }
    public string ExternalId { get; set; } = default!;
    public string Name { get; set; } = default!;
    public DeviceType Type { get; set; }
    public DeviceStatus Status { get; set; }
    public string? Location { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}