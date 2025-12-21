namespace server.models;

public class AvatarUser
{
    public Guid Id { get; set; }
    
    public string UserId { get; set; }
    public User User { get; set; }
    
    public string avatarPath { get; set; }
}