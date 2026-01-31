using System.Text.Json.Serialization;

namespace server.models;


public enum RoomStatus
{
    Active = 0,
    OutOfService = 1,
    OutOfOrder = 2,
    Maintenance = 3
}

