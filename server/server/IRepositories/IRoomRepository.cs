using server.DTO;
using server.models;

namespace server.IRepositories;

public interface IRoomRepository
{
    public Task<ResultDto> CreateRoom(RoomDto data);
    public Task<ResultDto> DeleteRoom(Guid id);
    public Task<ResultDto> PutInMaintenance(Guid id);
    public Task<PaginatedItemsDto<Room>> GetALlRooms(PaginationDto pagination);
    
    
}