using System.Runtime.InteropServices;

namespace server.DTO;

public class RoomDTO
{

    public string Name { get; set; } = "";
    public string RoomType { get; set; } = "";
    public int pricePerNight { get; set; } = 0;
    public string Description { get; set; } = "";
    public List<IFormFile> Photos { get; set; } = new List<IFormFile>();
}