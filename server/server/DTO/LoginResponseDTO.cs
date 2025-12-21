using server.models;

namespace server.DTO;

public class LoginResponseDTO
{
    public User FoundUser { get; set; }
    public string Token { get; set; }
    public string Error { get; set; }
}