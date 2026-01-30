using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TelemetryService.Domain.Entities;
using TelemetryService.Infrastructure.Persistence;
using TelemetryService.WebApi.Contracts.Devices;

namespace TelemetryService.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DevicesController : ControllerBase
{
    private readonly AppDbContext _db;

    public DevicesController(AppDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Создать устройство
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(DeviceResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateDeviceRequest request, CancellationToken ct)
    {
        var exists = await _db.Devices.AnyAsync(d => d.ExternalId == request.ExternalId, ct);
        if (exists)
        {
            return Conflict(new ProblemDetails
            {
                Title = "Device already exists",
                Detail = $"Device with ExternalId '{request.ExternalId}' already exists."
            });
        }

        var device = new Device
        {
            ExternalId = request.ExternalId.Trim(),
            Name = request.Name.Trim(),
            Type = request.Type,
            Location = string.IsNullOrWhiteSpace(request.Location) ? null : request.Location.Trim(),
        };

        _db.Devices.Add(device);
        await _db.SaveChangesAsync(ct);

        var response = Map(device);

        return CreatedAtAction(nameof(GetById), new { id = device.Id }, response);
    }

    /// <summary>
    /// Получить устройство по Id
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(DeviceResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken ct)
    {
        var device = await _db.Devices.AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == id, ct);

        if (device is null)
            return NotFound();

        return Ok(Map(device));
    }

    private static DeviceResponse Map(Device d) => new()
    {
        Id = d.Id,
        ExternalId = d.ExternalId,
        Name = d.Name,
        Type = d.Type,
        Status = d.Status,
        Location = d.Location,
        CreatedAt = d.CreatedAt
    };
}
