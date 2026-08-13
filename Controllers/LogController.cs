using bretts_services.Models;

namespace bretts_services.Controllers;

/// <summary>
/// Provides administrative access to application log entries.
/// </summary>
[Authorize(Roles = "Admin")]
[ApiController]
[Route("[controller]")]
public class LogController : ControllerBase
{
    private readonly ILogger<LogController> _logger;
    private readonly ILogService _logService;

    /// <summary>
    /// Initializes a new instance of the <see cref="LogController"/> class.
    /// </summary>
    /// <param name="logger">The controller logger.</param>
    /// <param name="logService">The service used to retrieve application logs.</param>
    public LogController(ILogger<LogController> logger, ILogService logService)
    {
        _logger = logger;
        _logService = logService;
    }

    /// <summary>
    /// Gets all application log entries.
    /// </summary>
    /// <remarks>
    /// Returns persisted Serilog entries ordered from oldest to newest. An authenticated user
    /// with the Admin role is required.
    /// </remarks>
    /// <returns>The stored application log entries.</returns>
    /// <response code="200">Returns the application log entries.</response>
    /// <response code="401">The request does not contain a valid JWT access token.</response>
    /// <response code="403">The authenticated user does not have the Admin role.</response>
    /// <response code="500">An unexpected server or database error occurred.</response>
    [HttpGet("logs")]
    [ProducesResponseType(typeof(List<Entities.Log>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Roles()
    {
        var logs = await _logService.GetLogs();

        return Ok(logs);
    }
}
