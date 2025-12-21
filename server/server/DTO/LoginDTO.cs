using System.ComponentModel.DataAnnotations;

namespace server.DTO;

public class LoginDTO
{
    [Required]
    [EmailAddress(ErrorMessage = "Your email is not valid")]
    public string Email { get; set; }
    
    [Required(ErrorMessage = "Enter password please")]
    public string Password { get; set; }
}