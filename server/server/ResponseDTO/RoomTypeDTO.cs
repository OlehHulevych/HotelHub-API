using server.models;

namespace server.ResponseDTO;

public class RoomTypeDTO
{
    public Guid Id { get; set; } 
    public string Name { get; set; }
    public int PricePerNight { get; set; }
    public IEnumerable<string> Photos { get; set; }
    public string Description { get; set; }
    public List<string> Norishment { get; set; }
    public List<string> Spa { get; set; }
    public List<string> View { get; set; }
    public int Capacity { get; set; }

    public RoomTypeDTO(Guid id, string name, int pricePerNight, IEnumerable<string> photos, string description,
        List<string> norishment, List<string> spa, List<string> view, int capacity)
    {
        Id = id;
        Name = name;
        PricePerNight = pricePerNight;
        Photos = photos;
        Description = description;
        Norishment = norishment;
        Spa = spa;
        View = view;
        Capacity = capacity;


    }
    
}
