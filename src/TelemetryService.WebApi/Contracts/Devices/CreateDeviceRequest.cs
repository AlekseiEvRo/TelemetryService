using System.ComponentModel.DataAnnotations;
using TelemetryService.Domain.Enums;

namespace TelemetryService.WebApi.Contracts.Devices;

public class CreateDeviceRequest
{
    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string ExternalId { get; set; } = default!;

    [Required]
    [StringLength(200, MinimumLength = 1)]
    public string Name { get; set; } = default!;

    public DeviceType Type { get; set; } = DeviceType.Sensor;

    [StringLength(200)]
    public string? Location { get; set; }
}