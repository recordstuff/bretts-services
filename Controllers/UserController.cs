using bretts_services.Models;
using bretts_services.Models.ViewModels;

namespace bretts_services.Controllers;

/// <summary>
/// Authenticates users and provides administrative user-management operations.
/// </summary>
[Authorize(Roles = "Admin")]
[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly IUserService _userService;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserController"/> class.
    /// </summary>
    /// <param name="logger">The controller logger.</param>
    /// <param name="userService">The service used for authentication and user management.</param>
    public UserController(ILogger<UserController> logger, IUserService userService)
    {
        _logger = logger;
        _userService = userService;
    }

    /// <summary>
    /// Authenticates a user and creates a JWT login session.
    /// </summary>
    /// <remarks>
    /// This is the only anonymous user endpoint. Submit a registered email address and password,
    /// then use the returned token as a bearer token for Admin-protected routes.
    /// </remarks>
    /// <param name="userCredentials">The email address and password used to authenticate.</param>
    /// <returns>The authenticated user's display name, roles, token expiration, and JWT access token.</returns>
    /// <response code="200">Authentication succeeded and a login session was created.</response>
    /// <response code="400">The request body, email address, or password is missing.</response>
    /// <response code="401">The email address or password is incorrect.</response>
    /// <response code="500">An unexpected server or database error occurred.</response>
    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginSession), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Login(UserCredentials? userCredentials)
    {
        if (string.IsNullOrWhiteSpace(userCredentials?.Email)
         || string.IsNullOrWhiteSpace(userCredentials.Password))
        {
            return BadRequest();
        }

        var login = await _userService.Login(userCredentials);

        if (!string.IsNullOrWhiteSpace(login.Token))
        {
            return Ok(login);
        }

        return Unauthorized();
    }

    /// <summary>
    /// Gets a filtered page of users.
    /// </summary>
    /// <remarks>
    /// Results can be filtered by matching the email address or display name and by requiring a
    /// specific role. An authenticated user with the Admin role is required.
    /// </remarks>
    /// <param name="page">The one-based page number to return. The value must be at least 1.</param>
    /// <param name="pageSize">The maximum number of users to return in the page.</param>
    /// <param name="searchText">Optional text matched case-insensitively against email addresses and display names.</param>
    /// <param name="roleFilter">Optional role filter. Use <see cref="Roles.Any"/> to include every role.</param>
    /// <param name="sortColumn">The user column to sort by. Defaults to display name.</param>
    /// <param name="sortDirection">The direction used to sort the selected column.</param>
    /// <returns>A page of user summaries and pagination metadata.</returns>
    /// <response code="200">Returns the requested page of users.</response>
    /// <response code="400">The page number is less than 1.</response>
    /// <response code="401">The request does not contain a valid JWT access token.</response>
    /// <response code="403">The authenticated user does not have the Admin role.</response>
    /// <response code="500">An unexpected server or database error occurred.</response>
    [HttpGet("users")]
    [ProducesResponseType(typeof(PaginationResult<UserSummary>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Users(int page, int pageSize, string? searchText = null, Roles roleFilter = Roles.Any,
        UsersSortColumn sortColumn = UsersSortColumn.DisplayName, SortDirection sortDirection = SortDirection.Ascending)
    {
        if (page < 1)
        {
            return BadRequest("Page must be greater than 1.");
        }

        var paginationResult = await _userService.GetUsers(page, pageSize, searchText, roleFilter, sortColumn, sortDirection);

        return Ok(paginationResult);
    }

    /// <summary>
    /// Gets one user by public identifier.
    /// </summary>
    /// <param name="guid">The public user identifier.</param>
    /// <returns>The matching user's details and assigned roles.</returns>
    /// <response code="200">Returns the matching user.</response>
    /// <response code="401">The request does not contain a valid JWT access token.</response>
    /// <response code="403">The authenticated user does not have the Admin role.</response>
    /// <response code="404">No user has the supplied identifier.</response>
    /// <response code="500">An unexpected server or database error occurred.</response>
    [HttpGet("user/{guid}")]
    [ProducesResponseType(typeof(UserDetail), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Users(Guid guid)
    {
        var user = await _userService.GetUser(guid);

        if (user is null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    /// <summary>
    /// Creates a user.
    /// </summary>
    /// <remarks>
    /// Email addresses are normalized to lowercase before storage. The request must include an
    /// email address, password, display name, and at least one valid role.
    /// </remarks>
    /// <param name="newUser">The user details, password, and role assignments to create.</param>
    /// <returns>The created user without password data.</returns>
    /// <response code="201">The user was created successfully.</response>
    /// <response code="400">A required email address, password, or display name is missing.</response>
    /// <response code="401">The request does not contain a valid JWT access token.</response>
    /// <response code="403">The authenticated user does not have the Admin role.</response>
    /// <response code="409">A user with the supplied email address or identifier already exists.</response>
    /// <response code="500">An unexpected server or database error occurred.</response>
    [HttpPost("insert")]
    [ProducesResponseType(typeof(UserDetail), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Insert(UserNew newUser)
    {
        if (string.IsNullOrWhiteSpace(newUser.Email)
         || string.IsNullOrWhiteSpace(newUser.Password)
         || string.IsNullOrWhiteSpace(newUser.DisplayName))
        {
            return BadRequest();
        }

        var addedUser = await _userService.InsertUser(newUser);
        
        if (addedUser is not null)
        {
            return Created(null as string, addedUser);
        };

        return Conflict();
    }

    /// <summary>
    /// Deletes a user by public identifier.
    /// </summary>
    /// <param name="guid">The public identifier of the user to delete.</param>
    /// <returns><see langword="true"/> when the user is deleted.</returns>
    /// <response code="200">The user was deleted successfully.</response>
    /// <response code="400">The supplied identifier is empty.</response>
    /// <response code="401">The request does not contain a valid JWT access token.</response>
    /// <response code="403">The authenticated user does not have the Admin role.</response>
    /// <response code="404">No user has the supplied identifier.</response>
    /// <response code="500">An unexpected server or database error occurred.</response>
    [HttpDelete("delete/{guid}")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete(Guid guid)
    {
        if (guid == Guid.Empty)
        {
            return BadRequest();
        }

        if (await _userService.DeleteUser(guid))
        {
            return Ok(true);
        };

        return NotFound();
    }

    /// <summary>
    /// Updates an existing user.
    /// </summary>
    /// <remarks>
    /// Updates the user's email address, display name, phone number, and role assignments. This
    /// action does not change the user's password.
    /// </remarks>
    /// <param name="userDetail">The complete editable user details and role assignments.</param>
    /// <returns>The updated user details.</returns>
    /// <response code="200">The user was updated successfully.</response>
    /// <response code="400">The request is invalid or the user does not exist.</response>
    /// <response code="401">The request does not contain a valid JWT access token.</response>
    /// <response code="403">The authenticated user does not have the Admin role.</response>
    /// <response code="500">An unexpected server or database error occurred.</response>
    [HttpPost("update")]
    [ProducesResponseType(typeof(UserDetail), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update(UserDetail userDetail)
    {
        if (string.IsNullOrWhiteSpace(userDetail.Email)
         || string.IsNullOrWhiteSpace(userDetail.DisplayName)
         || userDetail.Guid == Guid.Empty)
        {
            return BadRequest();
        }

        var updatedUser = await _userService.UpdateUser(userDetail);

        if (updatedUser == null)
        {
            return BadRequest();
        };

        return Ok(updatedUser);
    }
}
