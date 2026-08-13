namespace bretts_services.Controllers;

/// <summary>
/// Provides administrative access to the roles that can be assigned to users.
/// </summary>
[Authorize(Roles = "Admin")]
[ApiController]
[Route("[controller]")]
public class RoleController : ControllerBase
{
    private readonly ILogger<RoleController> _logger;
    private readonly IRoleService _roleService;

    /// <summary>
    /// Initializes a new instance of the <see cref="RoleController"/> class.
    /// </summary>
    /// <param name="logger">The controller logger.</param>
    /// <param name="roleService">The service used to retrieve roles.</param>
    public RoleController(ILogger<RoleController> logger, IRoleService roleService)
    {
        _logger = logger;
        _roleService = roleService;
    }

    /// <summary>
    /// Gets all assignable user roles.
    /// </summary>
    /// <remarks>
    /// Returns role names and public role identifiers ordered by role name. An authenticated
    /// user with the Admin role is required.
    /// </remarks>
    /// <returns>The roles available for user assignment.</returns>
    /// <response code="200">Returns the available roles.</response>
    /// <response code="401">The request does not contain a valid JWT access token.</response>
    /// <response code="403">The authenticated user does not have the Admin role.</response>
    /// <response code="500">An unexpected server or database error occurred.</response>
    [HttpGet("roles")]
    [ProducesResponseType(typeof(List<ViewModels.NameGuidPair>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Roles()
    {
        var roles = await _roleService.GetRoles();

        return Ok(roles);
    }
}
