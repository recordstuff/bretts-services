using Microsoft.AspNetCore.Authorization;

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
    [HttpPost("login", Name = "Login")]
    public async Task<IActionResult> Login(UserCredentials userCredentials)
    {
        if (userCredentials == null
        || string.IsNullOrWhiteSpace(userCredentials.Email)
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

    [HttpPost(Name = "AddUser")]
    public async Task<IActionResult> Add(UserCredentials userCredentials)
    {
        if (userCredentials == null
        || string.IsNullOrWhiteSpace(userCredentials.Email)
        || string.IsNullOrWhiteSpace(userCredentials.Password))
        {
            return BadRequest();
        }

        if (await _userService.Add(userCredentials))
        {
            // TODO: return user
            return Created(null as string, string.Empty);
        };

        return Conflict();
    }
}
