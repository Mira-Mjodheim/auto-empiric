using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AutoEmpiric.Core;
using AutoEmpiric.Core.Models;

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
            ValidationResult result = await _orchestrator.ExecuteMissionAsync(request.ProblemDescription);
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
[WARNING] --raw-output is enabled. Model output is not sanitized and may contain harmful ANSI sequences (e.g. for phishing or command injection). Use --accept-raw-output-risk to suppress this warning.