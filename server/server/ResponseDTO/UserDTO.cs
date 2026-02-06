namespace server.ResponseDTO;

public class UserDTO
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string? Position { get; set; }
    public Boolean? OnDuty { get; set; }
    public string? Photo { get; set; }
    public bool Banned { get; set; }

    public UserDTO(string id, string name, string email, string? position, Boolean? onDuty, string? photo, bool banned)
    {
        Id = id;
        Name = name;
        Email = email;
        Position = position;
        OnDuty = onDuty;
        Photo = photo;
        Banned = banned;

    }
}