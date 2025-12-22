namespace server.DTO;

public class RoomTypeDTO
{
    public string Name { get; set; } = "";
    public int pricePerNight { get; set; } = 0;
    public string Description { get; set; } = "";
    public List<string> Norishment { get; set; } = new List<string>();
    public List<string> Spa { get; set; } = new List<string>();
    public List<string> View { get; set; } = new List<string>();
    public int Capacity { get; set; } = 1;
    public List<IFormFile> Photos { get; set; } = new List<IFormFile>();
}