using bretts_services.Models;

namespace bretts_services.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly IUserService _userService;

    public UserController(ILogger<UserController> logger, IUserService userService)
    {
        _logger = logger;
        _userService = userService;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(UserCredentials? userCredentials)
    {
        if (string.IsNullOrWhiteSpace(userCredentials?.Email)
         || string.IsNullOrWhiteSpace(userCredentials.Password))
        {
            return BadRequest();
        }

        var token = await _userService.Login(userCredentials);

        if (!string.IsNullOrWhiteSpace(token))
        {
            return Ok(token);
        }

        return Unauthorized();
    }

    [HttpGet("users")]
    public async Task<IActionResult> Users(int page, int pageSize, string? searchText = null, Roles roleFilter = Roles.Any)
    {
        if (page < 1)
        {
            return BadRequest("Page must be greater than 1.");
        }

        var paginationResult = await _userService.GetUsers(page, pageSize, searchText, roleFilter);

        return Ok(paginationResult);
    }

    [HttpPost]
    public async Task<IActionResult> Add(NewUser? newUser)
    {
        if (string.IsNullOrWhiteSpace(newUser?.Email)
         || string.IsNullOrWhiteSpace(newUser.Password))
        {
            return BadRequest();
        }

        if (await _userService.Add(newUser))
        {
            // TODO: return user?
            return Created(null as string, string.Empty);
        };

        return Conflict();
    }
}
