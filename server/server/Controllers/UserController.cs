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
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromForm] LoginDto data)
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
    
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPost("changePassword")]
    public async Task<IActionResult> ChangePassword([FromForm] ChnagePasswordDto model)
    {
        string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        Console.WriteLine("This is id: "+userId);
        if (userId == null)
        {
            return Unauthorized("The user is not authorized");
        }

        var result = await _userRepository.ChangeUserPassword(userId, model);
        if(!result.Result)
        {
            return BadRequest(result.Message);
        }

        return Ok(result);

    }

    [Authorize(Roles = "ADMIN")]
    [HttpPatch("promote/{id}")]
    public async Task<IActionResult> PromoteUser(string id)
    {
        var response = await _userRepository.PromoteUser(id);
        if (!response.Result)
        {
            return BadRequest(response.Message);
        }

        return Ok(response);
    }

    [Authorize]
    [HttpPost]
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
    
    [Authorize(Roles = "ADMIN")]
    [HttpDelete]
    public async Task<IActionResult> DeleteUser([FromQuery] string id)
    {
        var response = await _userRepository.DeleteUser(id);
        if (!response.Result)
        {
            return BadRequest(response.Message);
        }

        return Ok(response);
    }
    
    

    [Authorize(Roles = "ADMIN")]
    [HttpGet("All")]
    public async Task<IActionResult> GetAllUser([FromQuery] PaginationDto query)
    {
        var response = await _userRepository.GetAllUser(query.CurrentPage);
        return Ok(response);
    }
    
    
}