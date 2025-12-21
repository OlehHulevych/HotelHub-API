namespace server.DTO;

public class ReservationDTO
{
    public Guid RoomId { get; set; }
    public DateOnly CheckIn { get; set; }
    public DateOnly CheckOut { get; set; }
    
}