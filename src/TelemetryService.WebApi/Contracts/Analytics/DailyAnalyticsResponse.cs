namespace TelemetryService.WebApi.Contracts.Analytics;

public class DailyAnalyticsResponse
{
    public Guid DeviceId { get; set; }
    public DateTimeOffset From { get; set; }
    public DateTimeOffset To { get; set; }

    public List<DailyAnalyticsItem> Items { get; set; } = new();
}