namespace server.DTO;

public class UpdateReservationDto
{
    public DateOnly CheckIn { get; set; } = DateOnly.MinValue;
    public DateOnly CheckOut { get; set; } = DateOnly.MinValue;

}