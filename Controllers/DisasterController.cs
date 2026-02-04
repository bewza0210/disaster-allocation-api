using Microsoft.AspNetCore.Mvc;
using DisasterApi.Services;

namespace DisasterApi.Controllers;

[ApiController]
[Route("api")]
public class AssignmentsController : ControllerBase
{
    [HttpPost("areas")]
    public IActionResult AddAffectedAreas()
    {
        return Ok(new { message = "Adding allow affected areas" });
    }

    [HttpPost("trucks")]
    public IActionResult AddResourceTrucks()
    {
        return Ok(new { message = "Allow adding resources trucks" });
    }

    [HttpPost("assignments")]
    public IActionResult GenerateAssignments()
    {
        return Ok(new { message = "Generate assignments to redis cache" });
    }

    [HttpGet("assignments")]
    public IActionResult GetAssignments()
    {
        return Ok(new { message = "Get assignments in redis cache" });
    }

    [HttpDelete("assignments")]
    public IActionResult DeleteAssignments()
    {
        return Ok(new { message = "Delete assignments in redis cache" });
    }
}