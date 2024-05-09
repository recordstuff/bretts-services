using bretts_services.Models;

namespace bretts_services.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("[controller]")]
public class LogController : ControllerBase
{
    private readonly ILogger<LogController> _logger;
    private readonly ILogService _logService;

    public LogController(ILogger<LogController> logger, ILogService logService)
    {
        _logger = logger;
        _logService = logService;
    }

    [HttpGet("logs")]
    public async Task<IActionResult> Roles()
    {
        var logs = await _logService.GetLogs();

        return Ok(logs);
    }
}
