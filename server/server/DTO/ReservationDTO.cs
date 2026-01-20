namespace server.DTO;

public class ReservationDto
{
    public Guid TypeId { get; set; }
    public DateOnly CheckIn { get; set; }
    public DateOnly CheckOut { get; set; }
    
}