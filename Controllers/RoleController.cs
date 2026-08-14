using bretts_services.Models.ViewModels;

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
    /// <param name="roleService">The service used to manage roles.</param>
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
    [HttpGet("allroles")]
    [ProducesResponseType(typeof(List<NameGuidPair>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AllRoles()
    {
        var roles = await _roleService.GetRoles();

        return Ok(roles);
    }

    /// <summary>
    /// Gets a filtered page of roles.
    /// </summary>
    /// <remarks>
    /// Results can be filtered by matching the role name and sorted by public identifier or name.
    /// An authenticated user with the Admin role is required.
    /// </remarks>
    /// <param name="page">The one-based page number to return. The value must be at least 1.</param>
    /// <param name="pageSize">The maximum number of roles to return in the page.</param>
    /// <param name="searchText">Optional text matched case-insensitively against role names.</param>
    /// <param name="sortColumn">The role column to sort by. Defaults to name.</param>
    /// <param name="sortDirection">The direction used to sort the selected column.</param>
    /// <returns>A page of roles and pagination metadata.</returns>
    /// <response code="200">Returns the requested page of roles.</response>
    /// <response code="400">The page number is less than 1.</response>
    /// <response code="401">The request does not contain a valid JWT access token.</response>
    /// <response code="403">The authenticated user does not have the Admin role.</response>
    /// <response code="500">An unexpected server or database error occurred.</response>
    [HttpGet("roles")]
    [ProducesResponseType(typeof(PaginationResult<NameGuidPair>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Roles(int page, int pageSize, string? searchText = null,
        RolesSortColumn sortColumn = RolesSortColumn.Name, SortDirection sortDirection = SortDirection.Ascending)
    {
        if (page < 1)
        {
            return BadRequest("Page must be greater than 1.");
        }

        var roles = await _roleService.GetRoles(page, pageSize, searchText, sortColumn, sortDirection);

        return Ok(roles);
    }

    /// <summary>
    /// Gets one role by public identifier.
    /// </summary>
    /// <param name="guid">The public role identifier.</param>
    /// <returns>The matching role.</returns>
    /// <response code="200">Returns the matching role.</response>
    /// <response code="400">The supplied identifier is empty.</response>
    /// <response code="401">The request does not contain a valid JWT access token.</response>
    /// <response code="403">The authenticated user does not have the Admin role.</response>
    /// <response code="404">No role has the supplied identifier.</response>
    /// <response code="500">An unexpected server or database error occurred.</response>
    [HttpGet("role/{guid}")]
    [ProducesResponseType(typeof(NameGuidPair), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetRole(Guid guid)
    {
        if (guid == Guid.Empty)
        {
            return BadRequest();
        }

        var role = await _roleService.GetRole(guid);

        if (role == null)
        {
            return NotFound();
        }

        return Ok(role);
    }

    /// <summary>
    /// Creates a role.
    /// </summary>
    /// <param name="role">The role name to create.</param>
    /// <returns>The created role.</returns>
    /// <response code="201">The role was created successfully.</response>
    /// <response code="400">The role name is missing.</response>
    /// <response code="401">The request does not contain a valid JWT access token.</response>
    /// <response code="403">The authenticated user does not have the Admin role.</response>
    /// <response code="409">A role with the supplied name already exists.</response>
    /// <response code="500">An unexpected server or database error occurred.</response>
    [HttpPost("insert")]
    [ProducesResponseType(typeof(NameGuidPair), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Insert(RoleNew role)
    {
        if (string.IsNullOrWhiteSpace(role.Name))
        {
            return BadRequest();
        }

        var changeResult = await _roleService.InsertRole(role);

        if (changeResult.Status == RoleChangeStatus.DuplicateName)
        {
            return Conflict();
        }

        return Created(null as string, changeResult.Role);
    }

    /// <summary>
    /// Updates an existing role.
    /// </summary>
    /// <param name="role">The complete editable role details.</param>
    /// <returns>The updated role.</returns>
    /// <response code="200">The role was updated successfully.</response>
    /// <response code="400">The request is invalid or the role does not exist.</response>
    /// <response code="401">The request does not contain a valid JWT access token.</response>
    /// <response code="403">The authenticated user does not have the Admin role.</response>
    /// <response code="409">A role with the supplied name already exists.</response>
    /// <response code="500">An unexpected server or database error occurred.</response>
    [HttpPost("update")]
    [ProducesResponseType(typeof(NameGuidPair), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update(NameGuidPair role)
    {
        if (!Guid.TryParse(role.Guid, out _) || string.IsNullOrWhiteSpace(role.Name))
        {
            return BadRequest();
        }

        var changeResult = await _roleService.UpdateRole(role);

        if (changeResult.Status == RoleChangeStatus.DuplicateName)
        {
            return Conflict();
        }

        if (changeResult.Status == RoleChangeStatus.RoleNotFound)
        {
            return BadRequest();
        }

        return Ok(changeResult.Role);
    }

    /// <summary>
    /// Deletes a role by public identifier.
    /// </summary>
    /// <param name="guid">The public identifier of the role to delete.</param>
    /// <returns><see langword="true"/> when the role is deleted.</returns>
    /// <response code="200">The role was deleted successfully.</response>
    /// <response code="400">The supplied identifier is empty.</response>
    /// <response code="401">The request does not contain a valid JWT access token.</response>
    /// <response code="403">The authenticated user does not have the Admin role.</response>
    /// <response code="404">No role has the supplied identifier.</response>
    /// <response code="409">The role is assigned to one or more users.</response>
    /// <response code="500">An unexpected server or database error occurred.</response>
    [HttpDelete("delete/{guid}")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete(Guid guid)
    {
        if (guid == Guid.Empty)
        {
            return BadRequest();
        }

        var changeResult = await _roleService.DeleteRole(guid);

        if (changeResult.Status == RoleChangeStatus.RoleNotFound)
        {
            return NotFound();
        }

        if (changeResult.Status == RoleChangeStatus.RoleInUse)
        {
            return Conflict();
        }

        return Ok(true);
    }
}
