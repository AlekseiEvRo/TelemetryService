using System.ComponentModel.DataAnnotations;

namespace TelemetryService.WebApi.Contracts.Telemetry;

public class TelemetryIngestRequest
{
    [Required]
    [MinLength(1)]
    public List<TelemetryIngestItem> Items { get; set; } = new();
}