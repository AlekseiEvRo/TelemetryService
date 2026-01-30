namespace TelemetryService.WebApi.Contracts.Telemetry;

public class TelemetryIngestResponse
{
    public int Received { get; set; }
    public int Inserted { get; set; }
    public int UnknownDevices { get; set; }
    public List<string> UnknownDeviceExternalIds { get; set; } = new();
}