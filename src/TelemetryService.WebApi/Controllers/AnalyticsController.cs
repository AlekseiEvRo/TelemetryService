using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TelemetryService.Infrastructure.Persistence;
using TelemetryService.WebApi.Contracts.Analytics;


namespace TelemetryService.WebApi.Controllers;

[ApiController]
[Route("api/devices/{deviceId:guid}/analytics")]
public class AnalyticsController : ControllerBase
{
    private readonly AppDbContext _db;

    public AnalyticsController(AppDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Daily-аналитика по устройству: avg/min/max температуры, средний заряд, кол-во записей, online ratio
    /// </summary>
    /// <param name="from">ISO 8601, например 2026-01-01T00:00:00Z</param>
    /// <param name="to">ISO 8601, например 2026-02-01T00:00:00Z (верхняя граница EXCLUSIVE)</param>
    [HttpGet("daily")]
    [ProducesResponseType(typeof(DailyAnalyticsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDaily(
        [FromRoute] Guid deviceId,
        [FromQuery] DateTimeOffset from,
        [FromQuery] DateTimeOffset to,
        CancellationToken ct)
    {
        if (from == default || to == default)
        {
            ModelState.AddModelError("from/to", "Both 'from' and 'to' query parameters are required.");
            return ValidationProblem(ModelState);
        }

        if (to <= from)
        {
            ModelState.AddModelError("to", "'to' must be greater than 'from'.");
            return ValidationProblem(ModelState);
        }

        var deviceExists = await _db.Devices.AsNoTracking().AnyAsync(d => d.Id == deviceId, ct);
        if (!deviceExists) return NotFound();

        var query = _db.TelemetryRecords.AsNoTracking()
            .Where(t => t.DeviceId == deviceId && t.Timestamp >= from && t.Timestamp < to);

        var rows = await query
            .GroupBy(t => t.Timestamp.UtcDateTime.Date)   // <- ключ: DateTime (00:00:00 UTC)
            .Select(g => new
            {
                Day = g.Key,
                RecordsCount = g.Count(),

                TempAvg = g.Where(x => x.Temperature != null).Select(x => x.Temperature!.Value).Average(),
                TempMin = g.Where(x => x.Temperature != null).Select(x => x.Temperature!.Value).Min(),
                TempMax = g.Where(x => x.Temperature != null).Select(x => x.Temperature!.Value).Max(),

                BatteryAvg = g.Where(x => x.BatteryLevel != null).Select(x => (double)x.BatteryLevel!.Value).Average(),

                OnlineKnownCount = g.Count(x => x.IsOnline != null),
                OnlineTrueCount = g.Count(x => x.IsOnline == true)
            })
            .OrderBy(x => x.Day)
            .ToListAsync(ct);

        var result = new DailyAnalyticsResponse
        {
            DeviceId = deviceId,
            From = from,
            To = to,
            Items = rows.Select(x => new DailyAnalyticsItem
            {
                Date = DateOnly.FromDateTime(x.Day),

                TemperatureAvg = (x.TempAvg == 0m && x.TempMin == 0m && x.TempMax == 0m) ? null : x.TempAvg,
                TemperatureMin = (x.TempMin == 0m && x.TempAvg == 0m && x.TempMax == 0m) ? null : x.TempMin,
                TemperatureMax = (x.TempMax == 0m && x.TempAvg == 0m && x.TempMin == 0m) ? null : x.TempMax,

                BatteryAvg = x.BatteryAvg == 0d ? null : x.BatteryAvg,

                RecordsCount = x.RecordsCount,

                OnlineRatio = x.OnlineKnownCount == 0
                    ? null
                    : (double)x.OnlineTrueCount / x.OnlineKnownCount
            }).ToList()
        };

        return Ok(result);
    }
}
