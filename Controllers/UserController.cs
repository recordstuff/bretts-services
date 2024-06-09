using bretts_services.Models;
using bretts_services.Models.ViewModels;

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

        var login = await _userService.Login(userCredentials);

        if (!string.IsNullOrWhiteSpace(login.Token))
        {
            return Ok(login);
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

    [HttpGet("user/{guid}")]
    public async Task<IActionResult> Users(Guid guid)
    {
        var user = await _userService.GetUser(guid);

        if (user is null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [HttpPost("insert")]
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

    [HttpPost("add")]
    public async Task<IActionResult> Add(UserNew newUser)
    {
        if (string.IsNullOrWhiteSpace(newUser.Email)
         || string.IsNullOrWhiteSpace(newUser.Password)
         || string.IsNullOrWhiteSpace(newUser.DisplayName))
        {
            return BadRequest();
        }

        if (await _userService.Add(newUser))
        {
            return Created(null as string, string.Empty);
        };

        return Conflict();
    }

    [HttpPost("update")]
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
