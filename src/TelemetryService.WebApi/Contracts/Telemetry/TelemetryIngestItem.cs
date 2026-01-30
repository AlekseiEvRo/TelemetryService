using System.ComponentModel.DataAnnotations;

namespace TelemetryService.WebApi.Contracts.Telemetry;

public class TelemetryIngestItem
{
    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string DeviceExternalId { get; set; } = default!;

    [Required]
    public DateTimeOffset Timestamp { get; set; }

    public decimal? Temperature { get; set; }

    [Range(0, 100)]
    public int? BatteryLevel { get; set; }

    public int? SignalRssi { get; set; }

    public bool? IsOnline { get; set; }
}