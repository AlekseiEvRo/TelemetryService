using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TelemetryService.Domain.Entities;
using TelemetryService.Infrastructure.Persistence;
using TelemetryService.WebApi.Contracts.Telemetry;

namespace TelemetryService.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TelemetryController : ControllerBase
{
    private const int MaxBatchSize = 1000;
    private readonly AppDbContext _db;

    public TelemetryController(AppDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Batch ingest телеметрии (до 1000 записей за запрос)
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(TelemetryIngestResponse), StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Ingest([FromBody] TelemetryIngestRequest request, CancellationToken ct)
    {
        if (request.Items.Count > MaxBatchSize)
        {
            ModelState.AddModelError(nameof(request.Items), $"Batch size must be <= {MaxBatchSize}.");
            return ValidationProblem(ModelState);
        }

        foreach (var item in request.Items)
            item.DeviceExternalId = item.DeviceExternalId.Trim();

        var extIds = request.Items
            .Select(x => x.DeviceExternalId)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct()
            .ToList();

        var devices = await _db.Devices.AsNoTracking()
            .Where(d => extIds.Contains(d.ExternalId))
            .Select(d => new { d.Id, d.ExternalId })
            .ToListAsync(ct);

        var map = devices.ToDictionary(x => x.ExternalId, x => x.Id);

        var unknown = extIds.Where(id => !map.ContainsKey(id)).ToList();
        if (unknown.Count > 0)
        {
            return BadRequest(new TelemetryIngestResponse
            {
                Received = request.Items.Count,
                Inserted = 0,
                UnknownDevices = unknown.Count,
                UnknownDeviceExternalIds = unknown
            });
        }

        var entities = request.Items.Select(x => new TelemetryRecord
        {
            DeviceId = map[x.DeviceExternalId],
            Timestamp = x.Timestamp.UtcDateTime == default ? x.Timestamp : x.Timestamp, // оставим как есть
            Temperature = x.Temperature,
            BatteryLevel = x.BatteryLevel,
            SignalRssi = x.SignalRssi,
            IsOnline = x.IsOnline
        }).ToList();

        _db.TelemetryRecords.AddRange(entities);
        var inserted = await _db.SaveChangesAsync(ct);

        return Accepted(new TelemetryIngestResponse
        {
            Received = request.Items.Count,
            Inserted = inserted,
            UnknownDevices = 0,
            UnknownDeviceExternalIds = new()
        });
    }
}
