namespace server.models;

public class RoomType
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; }
    public List<Room> RoomList { get; set; } = new List<Room>();
    public int Quantity { get; set; }
    public int PricePerNight { get; set; }
    public List<Reservation> Reservations { get; set; }
    public ICollection<Photo> Photos { get; set; }
    public string Description { get; set; }
    public Detail Detail { get; set; }
}