using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using server.Data;
using server.DTO;
using server.Repository;
using server.Tools;

namespace server.Controllers;

/// <summary>
/// User endpoints (authentication, profile, admin management).
/// </summary>
/// <remarks>
/// Includes registration/login and authenticated user operations.
/// Some endpoints are restricted to ADMIN users.
/// </remarks>
[ApiController]
[Route("api/user")]
[Tags("User")]
public class UserController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly JwtTokenService _jwtTokenService;
    private readonly UserRepository _userRepository;

    public UserController(ApplicationDbContext context, JwtTokenService jwtTokenService, UserRepository userRepository)
    {
        _context = context;
        _jwtTokenService = jwtTokenService;
        _userRepository = userRepository;
    }

    /// <summary>
    /// Register a new user account.
    /// </summary>
    /// <param name="data">Registration data (multipart/form-data because it uses <c>[FromForm]</c>).</param>
    /// <returns>Result of the registration operation.</returns>
    /// <response code="200">User registered successfully.</response>
    /// <response code="400">Validation failed or registration failed.</response>
    [HttpPost("register")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromForm] RegisterDto data)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest("The body was wrong");
        }

        var response = await _userRepository.RegisterUser(data);

        if (!response.Result)
        {
            return BadRequest(response.Message);
        }

        return Ok(response);
    }

    /// <summary>
    /// Login and receive a JWT token.
    /// </summary>
    /// <param name="data">Login data (multipart/form-data because it uses <c>[FromForm]</c>).</param>
    /// <returns>JWT access token if credentials are valid.</returns>
    /// <response code="200">Login successful; returns JWT token.</response>
    /// <response code="400">Missing credentials or invalid credentials.</response>
    [HttpPost("login")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromForm] LoginDto data)
    {
        // Note: this checks both missing; if you want "either missing" use OR (||) instead of AND (&&).
        if (string.IsNullOrWhiteSpace(data.Email) && string.IsNullOrWhiteSpace(data.Password))
        {
            return BadRequest("Something is missing");
        }

        var result = await _userRepository.LoginUser(data);
        if (result.FoundUser == null || result.Token == null)
        {
            return BadRequest(result.Error);
        }

        return Ok(new { result.Token });
    }

    /// <summary>
    /// Get information about the currently logged-in user.
    /// </summary>
    /// <returns>The current user's profile data.</returns>
    /// <response code="200">User info returned successfully.</response>
    /// <response code="400">Request failed (repository returned an error).</response>
    /// <response code="401">Unauthorized (missing/invalid JWT).</response>
    [Authorize]
    [HttpGet("me")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Me()
    {
        var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (id == null)
        {
            return Unauthorized(new { message = "The user is not loged" });
        }

        ResultDto response = await _userRepository.GetUserInformation(id);
        if (!response.Result)
        {
            return BadRequest(response.Message);
        }

        return Ok(new { message = "The user is retrived", User = response.Item });
    }

    /// <summary>
    /// Change the password of the currently logged-in user.
    /// </summary>
    /// <param name="model">Password change data (multipart/form-data because it uses <c>[FromForm]</c>).</param>
    /// <returns>Result of the password change operation.</returns>
    /// <response code="200">Password changed successfully.</response>
    /// <response code="400">Change failed (wrong old password, validation error, etc.).</response>
    /// <response code="401">Unauthorized (missing/invalid JWT).</response>
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPost("changePassword")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ChangePassword([FromForm] ChnagePasswordDto model)
    {
        string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null)
        {
            return Unauthorized("The user is not authorized");
        }

        var result = await _userRepository.ChangeUserPassword(userId, model);
        if (!result.Result)
        {
            return BadRequest(result.Message);
        }

        return Ok(result);
    }

    /// <summary>
    /// Promote a user to Owner (owner only).
    /// </summary>
    /// <param name="id">User id to promote.</param>
    /// <returns>Result of the promotion operation.</returns>
    /// <response code="200">User promoted successfully.</response>
    /// <response code="400">Promotion failed (invalid id / user not found / etc.).</response>
    /// <response code="401">Unauthorized (missing/invalid JWT).</response>
    /// <response code="403">Forbidden (not an ADMIN).</response>
    [Authorize(Roles = "OWNER")]
    [HttpPost("promote/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> PromoteUser(string id, [FromForm] PromoteDTO data)
    {
        var response = await _userRepository.PromoteUser(id,data);
        if (!response.Result)
        {
            return BadRequest(response.Message);
        }

        return Ok(response);
    }

    /// <summary>
    /// Update the currently logged-in user's profile.
    /// </summary>
    /// <param name="data">Profile update data (multipart/form-data because it uses <c>[FromForm]</c>).</param>
    /// <returns>Result of the update operation.</returns>
    /// <response code="200">User updated successfully.</response>
    /// <response code="400">Update failed (validation error / repository error).</response>
    /// <response code="401">Unauthorized (missing/invalid JWT).</response>
    [Authorize]
    [HttpPost("update")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> EditUser([FromForm] EditUserDtO data)
    {
        var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(id))
        {
            return Unauthorized("The user is not authorized");
        }

        var resposne = await _userRepository.EditUser(data, id);
        if (!resposne.Result)
        {
            return BadRequest(resposne.Message);
        }

        return Ok(resposne);
    }

    /// <summary>
    /// Delete a user by id (admin only).
    /// </summary>
    /// <param name="id">User id (query parameter).</param>
    /// <returns>Result of the delete operation.</returns>
    /// <response code="200">User deleted successfully.</response>
    /// <response code="400">Delete failed (invalid id / user not found / etc.).</response>
    /// <response code="401">Unauthorized (missing/invalid JWT).</response>
    /// <response code="403">Forbidden (not an ADMIN).</response>
    [Authorize(Roles = "ADMIN")]
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteUser([FromQuery] string id)
    {
        var response = await _userRepository.DeleteUser(id);
        if (!response.Result)
        {
            return BadRequest(response.Message);
        }

        return Ok(response);
    }

    /// <summary>
    /// Get all users (admin only).
    /// </summary>
    /// <param name="query">Pagination parameters.</param>
    /// <returns>A paginated list of users.</returns>
    /// <response code="200">Users returned successfully.</response>
    /// <response code="401">Unauthorized (missing/invalid JWT).</response>
    /// <response code="403">Forbidden (not an ADMIN).</response>
    [Authorize(Roles = "ADMIN")]
    [HttpGet("All")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAllUser([FromQuery] PaginationDto query)
    {
        var response = await _userRepository.GetAllUser(query.CurrentPage);
        return Ok(response);
    }
}
