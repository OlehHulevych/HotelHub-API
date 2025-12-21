namespace server.DTO;

public class UpdateRoomTypeDTO:RoomTypeDTO
{
    public List<IFormFile> newPhotos { get; set; } = new List<IFormFile>();
    public List<string> deletedPhotos { get; set; } = new List<string>();
}