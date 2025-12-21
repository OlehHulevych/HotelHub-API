using server.DTO;

namespace server.IRepositories;

public interface IRoomTypeRepository
{
     Task<ResultDTO> AddRoomType(RoomTypeDTO data);
     Task<ResultDTO> RemoveRoomType(Guid id);
}