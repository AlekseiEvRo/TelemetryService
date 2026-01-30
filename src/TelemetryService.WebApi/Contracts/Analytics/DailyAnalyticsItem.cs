namespace TelemetryService.WebApi.Contracts.Analytics;

public class DailyAnalyticsItem
{
    public DateOnly Date { get; set; }

    public decimal? TemperatureAvg { get; set; }
    public decimal? TemperatureMin { get; set; }
    public decimal? TemperatureMax { get; set; }

    public double? BatteryAvg { get; set; }

    public int RecordsCount { get; set; }

    public double? OnlineRatio { get; set; }
}