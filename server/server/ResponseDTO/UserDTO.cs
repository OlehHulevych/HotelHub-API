namespace server.ResponseDTO;

public class UserDTO
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string? Position { get; set; }
    public Boolean? OnDuty { get; set; }
    public string Photo { get; set; }
    
}