namespace server.DTO;

public class RoomTypeDTO
{
    public string Name { get; set; }
    public int pricePerNight { get; set; } = 0;
    public string Description { get; set; } = "";
    public List<string> Norishment { get; set; }
    public List<string> Spa { get; set; }
    public List<string> View { get; set; }
    public int Capacity { get; set; }
    public List<IFormFile> Photos { get; set; } = new List<IFormFile>();
}