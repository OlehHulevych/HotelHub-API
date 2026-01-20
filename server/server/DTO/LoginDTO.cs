using System.ComponentModel.DataAnnotations;

namespace server.DTO;

public class LoginDto
{
    [Required]
    [EmailAddress(ErrorMessage = "Your email is not valid")]
    public required string Email { get; set; }
    
    [Required(ErrorMessage = "Enter password please")]
    public string Password { get; set; }
}