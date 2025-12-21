namespace server.models;

public class Reservation
{
    public Guid Id { get; set; }
    public string UserId { get; set; }
    public User? User { get; set; }
    public Guid RoomId { get; set; }
    public Room Room { get; set; }
    public DateOnly CheckInDate { get; set; }
    public DateOnly CheckOutDate { get; set; }
    public int TotalPrice { get; set; }
    public string Status { get; set; }

}