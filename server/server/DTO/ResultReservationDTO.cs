using server.models;

namespace server.DTO;

public class ResultReservationDto:ResultDto
{
    public Room? ReservedRoom { get; set; }
}