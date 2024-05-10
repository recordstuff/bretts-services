using bretts_services.Models.Entities;

namespace bretts_services.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("[controller]")]
public class TestController : ControllerBase
{
    private readonly ILogger<TestController> _logger;
    private readonly IHostApplicationLifetime _hostApplicationLifetime;
    private readonly BrettsAppContext _brettsAppContext;

    public TestController(ILogger<TestController> logger, IHostApplicationLifetime hostApplicationLifetime, BrettsAppContext brettsAppContext)
    {
        _logger = logger;
        _hostApplicationLifetime = hostApplicationLifetime;
        _brettsAppContext = brettsAppContext;
    }

    [HttpDelete("shutdown")]
    public IActionResult Shutdown()
    {
        _logger.LogCritical("Shutting down the server.");
        
        _hostApplicationLifetime.StopApplication();

        return Ok();
    }

    [HttpGet("throwerror")]
    public IActionResult ThrowError()
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(null, "Hello there yall!");

        return Ok();
    }

    [HttpGet("structuredlogentry")]
    public async Task<IActionResult> StructuredLogEntry()
    {
        var user = await _brettsAppContext.Users.FirstOrDefaultAsync();

        _logger.LogInformation("You are a superstar! {@superstar}", user);

        return Ok();
    }
}
