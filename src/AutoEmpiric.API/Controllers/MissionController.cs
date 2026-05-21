using System;
using System.Threading.Tasks;
using AutoEmpiric.Core;
using AutoEmpiric.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace AutoEmpiric.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MissionController : ControllerBase
{
    private readonly Orchestrator _orchestrator;

    public MissionController(Orchestrator orchestrator)
    {
        _orchestrator = orchestrator ?? throw new ArgumentNullException(nameof(orchestrator));
    }

    [HttpPost("submit")]
    public async Task<ActionResult<ValidationResult>> SubmitMission([FromBody] MissionRequest request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.ProblemDescription))
        {
            return BadRequest("The problem description is required.");
        }

        try
        {
            var result = await _orchestrator.ExecuteEmpiricalCycleAsync(request.ProblemDescription);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}

public class MissionRequest
{
    public string ProblemDescription { get; set; } = string.Empty;
}
