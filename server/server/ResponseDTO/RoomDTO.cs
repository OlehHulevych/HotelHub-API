using server.models;

namespace server.ResponseDTO;

public class RoomDTO
{

    public Guid Id { get; set; }
    public string Name { get; set; }
    public int Capacity { get; set; }
    public int Number { get; set; }
    public int PricePerNight { get; set; }
    public string Type { get; set; }
    public RoomStatus Status { get; set; }

    public RoomDTO(Guid id, string name, int capacity, int number, int pricePerNight, string type, RoomStatus status)
    {
        Id = id;
        Name = name;
        Capacity = capacity;
        Number = number;
        PricePerNight = pricePerNight;
        Type = type;
        Status = status;
    }
    
}