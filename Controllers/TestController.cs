namespace bretts_services.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("[controller]")]
public class TestController : ControllerBase
{
    private readonly ILogger<TestController> _logger;
    private readonly IHostApplicationLifetime _hostApplicationLifetime;

    public TestController(ILogger<TestController> logger, IHostApplicationLifetime hostApplicationLifetime)
    {
        _logger = logger;
        _hostApplicationLifetime = hostApplicationLifetime;
    }

    [HttpDelete("shutdown")]
    public IActionResult Shutdown()
    {
        _logger.LogCritical("Shutting down the server.");
        
        _hostApplicationLifetime.StopApplication();

        return Ok();
    }
}
