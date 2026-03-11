using Microsoft.AspNetCore.Mvc;

namespace BookRepositoryApi.Controllers;

[ApiController]
[Route("[controller]")]
public class HealthController : ControllerBase
{
    private readonly ILogger<HealthController> _logger;

    public HealthController(ILogger<HealthController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Health check endpoint for load balancer and monitoring
    /// </summary>
    /// <returns>200 OK if service is healthy</returns>
    [HttpGet]
    public IActionResult Get()
    {
        _logger.LogDebug("Health check requested");
        
        return Ok(new 
        { 
            status = "Healthy",
            timestamp = DateTime.UtcNow,
            service = "BookRepositoryApi"
        });
    }

    /// <summary>
    /// Readiness check endpoint
    /// </summary>
    /// <returns>200 OK if service is ready to accept traffic</returns>
    [HttpGet("ready")]
    public IActionResult Ready()
    {
        // Can add additional checks here (database connectivity, etc.)
        return Ok(new 
        { 
            status = "Ready",
            timestamp = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Liveness check endpoint
    /// </summary>
    /// <returns>200 OK if service is alive</returns>
    [HttpGet("live")]
    public IActionResult Live()
    {
        return Ok(new 
        { 
            status = "Alive",
            timestamp = DateTime.UtcNow
        });
    }
}
