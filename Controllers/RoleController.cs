namespace bretts_services.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("[controller]")]
public class RoleController : ControllerBase
{
    private readonly ILogger<RoleController> _logger;
    private readonly IRoleService _roleService;

    public RoleController(ILogger<RoleController> logger, IRoleService roleService)
    {
        _logger = logger;
        _roleService = roleService;
    }

    [HttpGet("roles")]
    public async Task<IActionResult> Roles()
    {
        var roles = await _roleService.GetRoles();

        return Ok(roles);
    }
}
