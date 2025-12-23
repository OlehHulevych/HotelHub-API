using System.Text.Json.Serialization;
using server.DTO;

namespace server.models;

public class Room
{
   
    public Guid Id { get; set; } = Guid.NewGuid();
    public int Number { get; set; }
    public Guid RoomTypeId { get; set; }
    public RoomType Type { get; set; }
    public List<Reservation> Reservations { get; set; }
    
}