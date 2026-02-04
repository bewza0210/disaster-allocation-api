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
    public IActionResult AddAffectedAreas([FromBody] Area area)
    {
        _service.AddAffectedArea(area);
        return Ok(new { message = "Adding allow affected areas" });
    }

    [HttpPost("trucks")]
    public IActionResult AddResourceTrucks([FromBody] Truck truck)
    {
        _service.AddResourceTruck(truck);
        return Ok(new { message = "Allow adding resources trucks" });
    }

    [HttpPost("assignments")]
    public IActionResult GenerateAssignments()
    {
        _service.GenerateAssignments();
        return Ok(new { message = "Generate assignments to redis cache" });
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