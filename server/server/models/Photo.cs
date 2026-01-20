using System.Text.Json.Serialization;

namespace server.models;

public class Photo
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid RoomTypeId { get; set; }
    
    
    public RoomType RoomType { get; set; }
    public string Uri { get; set; }
    public string PublicId { get; set; }
}