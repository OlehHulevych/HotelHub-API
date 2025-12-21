using System.Text.Json.Serialization;
using server.DTO;

namespace server.models;

public class Room
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; }
    public Guid RoomTypeId { get; set; }
    public RoomType Type { get; set; }
    public int PricePerNight { get; set; }
    public string Description { get; set; }
    
    
    public ICollection<Photo> Photos { get; set; }
    public List<Reservation> Reservations { get; set; }
    
}