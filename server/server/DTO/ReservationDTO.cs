namespace server.DTO;

public class ReservationDTO
{
    public Guid TypeId { get; set; }
    public DateOnly CheckIn { get; set; }
    public DateOnly CheckOut { get; set; }
    
}