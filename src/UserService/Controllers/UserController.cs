using Microsoft.AspNetCore.Mvc;
using Shared.Models;

namespace UserService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;

    public UserController(ILogger<UserController> logger)
    {
        _logger = logger;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(Guid id)
    {
        try
        {
            var user = new { id, email = "user@example.com", name = "Sample User" };
            _logger.LogInformation("Retrieved user {UserId}", id);

            return Ok(ApiResponse<object>.SuccessResponse(user, "User retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user {UserId}", id);
            return StatusCode(500, ApiResponse<object>.ErrorResponse("Error retrieving user"));
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        try
        {
            var users = new[]
            {
                new { id = Guid.NewGuid(), email = "user1@example.com", name = "User 1" },
                new { id = Guid.NewGuid(), email = "user2@example.com", name = "User 2" }
            };

            return Ok(ApiResponse<object>.SuccessResponse(users, "Users retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving users");
            return StatusCode(500, ApiResponse<object>.ErrorResponse("Error retrieving users"));
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
    {
        try
        {
            var newUser = new { id = Guid.NewGuid(), email = request.Email, name = request.Name };
            _logger.LogInformation("User created: {Email}", request.Email);

            return CreatedAtAction(nameof(GetUser), new { id = newUser.id },
                ApiResponse<object>.SuccessResponse(newUser, "User created successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user");
            return StatusCode(500, ApiResponse<object>.ErrorResponse("Error creating user"));
        }
    }
}

public class CreateUserRequest
{
    public string Email { get; set; }
    public string Name { get; set; }
}
