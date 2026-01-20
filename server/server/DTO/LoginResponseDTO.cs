using server.models;

namespace server.DTO;

public class LoginResponseDto
{
    public User FoundUser { get; set; }
    public string Token { get; set; }
    public string Error { get; set; }
}