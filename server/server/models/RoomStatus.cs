using System.Text.Json.Serialization;

namespace server.models;


public enum RoomStatus
{
    Free = 0,
    OutOfService = 1,
    Occupied = 2,
    Maintenance = 3,
    None = 4
}

