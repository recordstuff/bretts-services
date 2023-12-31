namespace bretts_services.Controllers;

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

    [HttpGet(Name = "Index")]
    public IActionResult Index()
    {
        return Ok();
    }

    [HttpPost(Name = "Login")]
    public IActionResult Login(UserCredintials userCredintials)
    {
        if (userCredintials == null
        || string.IsNullOrWhiteSpace(userCredintials.Email)
        || string.IsNullOrWhiteSpace(userCredintials.Password))
        {
            return BadRequest();
        }

        return Ok(_userService.Login(userCredintials));
    }

    [HttpPost(Name = "Add")]
    public IActionResult Add(UserCredintials userCredintials)
    {
        if (userCredintials == null
        || string.IsNullOrWhiteSpace(userCredintials.Email)
        || string.IsNullOrWhiteSpace(userCredintials.Password))
        {
            return BadRequest();
        }

        // u r here

        _userService.Add(userCredintials);

        return Ok();
    }
}
