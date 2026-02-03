using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using server.models;

namespace server.DTO;

public class RegisterDto
{
    
    [Required(ErrorMessage = "Name is mandatory.")]
    public string Name { get; set; }
    
    [Required(ErrorMessage = "The email is required")]
    [EmailAddress]
    public required string Email { get; set; }
    
    [Required(ErrorMessage = "The password is required")]
    public string Password { get; set; }
    public string? Role { get; set; } = Roles.User;
    public string? Position { get; set; } = null;
    
    public IFormFile? Avatar { get; set; } = null;
}