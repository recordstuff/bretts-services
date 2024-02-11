using Microsoft.AspNetCore.Authorization;

namespace bretts_services.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class RoleController : ControllerBase
{
    private readonly ILogger<RoleController> _logger;
    private readonly IUserService _userService;

    public RoleController(ILogger<RoleController> logger, IUserService userService)
    {
        _logger = logger;
        _userService = userService;
    }

    [HttpPost(Name = "AddRole")]
    //[Authorize(Roles = "Admin")]
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
