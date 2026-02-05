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
        var (statusCode, success, message) = await _service.AddAffectedArea(area);
        return StatusCode(statusCode, new { success, message });
    }

    [HttpPost("trucks")]
    public async Task<IActionResult> AddResourceTrucks([FromBody] Truck truck)
    {
        var (statusCode, success, message) = await _service.AddResourceTruck(truck);
        return StatusCode(statusCode, new {  success, message });
    }

    [HttpPost("assignments")]
    public async Task<IActionResult> GenerateAssignments()
    {
        var (statusCode, success, message) = await _service.GenerateAssignments();
        return StatusCode(statusCode, new { success, message });
    }

    [HttpGet("assignments")]
    public async Task<IActionResult> GetAssignments()
    {
        var (statusCode, success, message, data) = await _service.GetAssignments();
        return StatusCode(statusCode, new { success, message, data });
    }

    [HttpDelete("assignments")]
    public async Task<IActionResult> DeleteAssignments()
    {
        var (statusCode, success, message) = await _service.DeleteAssignments();
        return StatusCode(statusCode, new { success, message });
    }
}