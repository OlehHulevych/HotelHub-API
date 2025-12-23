using server.DTO;
using server.models;
using server.Tools;

namespace server.IRepositories;

public interface IRoomRepository
{
    public Task<ResultDTO> createRoom(RoomDTO data);
    public Task<ResultDTO> deleteRoom(Guid id);
    public Task<PaginatedItemsDTO<Room>> getALlRooms(PaginationDTO pagination);
    
    
}