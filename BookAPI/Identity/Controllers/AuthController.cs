using BookAPI.Identity.Contracts;
using BookAPI.Identity.Services;

namespace BookAPI.Identity.Controllers;

public class AuthController(IAuthService authService) : ControllerBase
{
    /// <summary>
    /// Registers a new user.
    /// </summary>
    /// <param name="request">FirstName, LastName, Email and Password.</param>
    /// <returns>The newly created user's ID.</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /api/auth/register
    ///     {
    ///         "firstName": "Jane",
    ///         "lastName": "Doe",
    ///         "email": "jane@example.com",
    ///         "password": "P@ssw0rd!"
    ///     }
    /// </remarks>
    /// <response code="201">User registered successfully.</response>
    /// <response code="400">Validation error or email already in use.</response>
    /// <response code="500">Internal server error.</response>
    [HttpPost("register")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RegisterAsync([FromBody] RegisterRequest request)
    {
        var newUserId = await authService.RegisterAsync(request, HttpContext.RequestAborted);
        
        return Created(string.Empty, newUserId);
    }
    
    /// <summary>
    /// Authenticates a user and returns a JWT.
    /// </summary>
    /// <param name="request">Email and Password.</param>
    /// <returns>User info plus JWT token.</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /api/auth/login
    ///     {
    ///         "email": "jane@example.com",
    ///         "password": "P@ssw0rd!"
    ///     }
    /// </remarks>
    /// <response code="200">Authentication successful.</response>
    /// <response code="400">Invalid credentials.</response>
    /// <response code="500">Internal server error.</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthenticationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request)
    {
        var authResponse = await authService.AuthenticateAsync(request, HttpContext.RequestAborted);
        return Ok(authResponse);
    }
}