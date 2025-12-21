using System.Text.Json.Serialization;

namespace server.models;

public class Photo
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid RoomId { get; set; }
    
    
    public Room Room { get; set; }
    public string Uri { get; set; }
    public string public_id { get; set; }
}