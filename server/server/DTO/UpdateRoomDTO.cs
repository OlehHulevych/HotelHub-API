namespace server.DTO;

public class UpdateRoomDto:RoomDto
{
    public List<IFormFile> NewPhotos { get; set; } = new List<IFormFile>();
    public List<string> DeletedPhotos { get; set; } = new List<string>();
}