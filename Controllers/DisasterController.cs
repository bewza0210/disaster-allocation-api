using Microsoft.AspNetCore.Mvc;
using DisasterApi.Services;
using DisasterApi.Models;

namespace DisasterApi.Controllers;

[ApiController]
[Route("api")]
public class DisasterController : ControllerBase
{
    private readonly DisasterService _service;
    public DisasterController(DisasterService service)
    {
        _service = service;
    }

    [HttpPost("areas")]
    public async Task<IActionResult> AddAffectedAreas([FromBody] Area area)
    {
        var result = await _service.AddAffectedArea(area);
        if (!result.success) return Conflict(new { result.message });
        return Ok(new { result.message });
    }

    [HttpPost("trucks")]
    public async Task<IActionResult> AddResourceTrucks([FromBody] Truck truck)
    {
        var result = await _service.AddResourceTruck(truck);
        if (!result.success) return Conflict(new { result.message });
        return Ok(new { result.message});
    }

    [HttpPost("assignments")]
    public async Task<IActionResult> GenerateAssignments()
    {
        var result = await _service.GenerateAssignments();
        if (!result.success) return Conflict(new { result.message });
        return Ok(new { result.message });
    }

    [HttpGet("assignments")]
    public IActionResult GetAssignments()
    {
        _service.GetAssignments();
        return Ok(new { message = "Get assignments in redis cache" });
    }

    [HttpDelete("assignments")]
    public IActionResult DeleteAssignments()
    {
        _service.DeleteAssignments();
        return Ok(new { message = "Delete assignments in redis cache" });
    }
}