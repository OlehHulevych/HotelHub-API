using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using server.Data;

using server.DTO;
using server.Repository;
using server.Tools;

namespace server.Controllers;
[ApiController]
[Route("api/user")]
public class UserController : ControllerBase
{
    private ApplicationDbContext _context;

    private JwtTokenService _jwtTokenService;
    private UserRepository _userRepository;

    public UserController(ApplicationDbContext context, JwtTokenService jwtTokenService, UserRepository userRepository)
    {
        _context = context;
        _jwtTokenService = jwtTokenService;
        _userRepository = userRepository;
    }
    // GET

    [HttpPost("register")]
    public async Task<IActionResult> register([FromForm] RegisterDTO data)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest("The body was wrong");
        }

        var response = await _userRepository.RegisterUser(data);
        
        if (!response.result)
        {
            return BadRequest(response.Message);
        }

        return Ok(response);



    }
    [HttpPost("login")]
    public async Task<IActionResult> login([FromForm] LoginDTO data)
    {
        if (string.IsNullOrWhiteSpace(data.Email) && string.IsNullOrWhiteSpace(data.Password))
        {
            return BadRequest("Something is missing");
        }

        var result = await _userRepository.LoginUser(data);
        if (result.FoundUser == null || result.Token == null)
        {
            return BadRequest(result.Error);
        }
        
        
        
        return Ok(new {result.Token});
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> me()
    {
        var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (id == null)
        {
            return Unauthorized(new { message = "The user is not loged" });
        }

        ResultDTO response = await _userRepository.getUserInformation(id);
        if (!response.result)
        {
            return BadRequest(response.Message);
        }

        return Ok(new { message = "The user is retrived", User = response.Item });

    }
    
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPost("changePassword")]
    public async Task<IActionResult> ChangePassword([FromForm] ChnagePasswordDTO model)
    {
        string UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        Console.WriteLine("This is id: "+UserId);
        if (UserId == null)
        {
            return Unauthorized("The user is not authorized");
        }

        var result = await _userRepository.ChangeUserPassword(UserId, model);
        if(!result.result)
        {
            return BadRequest(result.Message);
        }

        return Ok(result);

    }

    [Authorize(Roles = "ADMIN")]
    [HttpPatch("promote/{id}")]
    public async Task<IActionResult> promoteUser(string id)
    {
        var response = await _userRepository.promoteUser(id);
        if (!response.result)
        {
            return BadRequest(response.Message);
        }

        return Ok(response);
    }
    
    [Authorize(Roles = "ADMIN")]
    [HttpDelete]
    public async Task<IActionResult> deleteUser([FromQuery] string id)
    {
        var response = await _userRepository.deleteUser(id);
        if (!response.result)
        {
            return BadRequest(response.Message);
        }

        return Ok(response);
    }

    [Authorize(Roles = "ADMIN")]
    [HttpGet("All")]
    public async Task<IActionResult> getAllUser([FromQuery] PaginationDTO query)
    {
        var response = await _userRepository.getAllUser(query.currentPage);
        return Ok(response);
    }
    
    
}