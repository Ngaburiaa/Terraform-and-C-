using BookRepositoryApi.Constants;
using BookRepositoryApi.Models;
using BookRepositoryApi.Models.Common;
using Microsoft.AspNetCore.Mvc;

namespace BookRepositoryApi.Controllers;

[ApiController]
[Route("[controller]")]
public class HealthController : ControllerBase
{
    private readonly ILogger<HealthController> _logger;

    // Initializes a new instance of the HealthController class.
    public HealthController(ILogger<HealthController> logger)
    {
        _logger = logger;
    }

    // Returns the overall service health status.
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<HealthStatusResponse>), StatusCodes.Status200OK)]
    public ActionResult<ApiResponse<HealthStatusResponse>> Get()
    {
        _logger.LogDebug("Health check requested");

        return Ok(CreateResponse("Healthy", "BookRepositoryApi"));
    }

    // Returns the readiness state for startup and orchestration checks.
    [HttpGet("ready")]
    [ProducesResponseType(typeof(ApiResponse<HealthStatusResponse>), StatusCodes.Status200OK)]
    public ActionResult<ApiResponse<HealthStatusResponse>> Ready() =>
        Ok(CreateResponse("Ready"));

    // Returns the liveness state for runtime monitoring.
    [HttpGet("live")]
    [ProducesResponseType(typeof(ApiResponse<HealthStatusResponse>), StatusCodes.Status200OK)]
    public ActionResult<ApiResponse<HealthStatusResponse>> Live() =>
        Ok(CreateResponse("Alive"));

    private static ApiResponse<HealthStatusResponse> CreateResponse(string status, string? service = null) =>
        new()
        {
            Success = true,
            Message = ResponseMessages.HealthCheckSucceeded,
            Data = new HealthStatusResponse
            {
                Status = status,
                TimestampUtc = DateTime.UtcNow,
                Service = service
            }
        };
}

