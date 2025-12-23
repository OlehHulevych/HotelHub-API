using server.models;

namespace server.DTO;

public class ResultReservationDTO:ResultDTO
{
    public Room? reservedRoom { get; set; }
}