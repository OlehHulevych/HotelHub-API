using server.DTO;
using server.models;

namespace server.IRepositories;

public interface IRoomTypeRepository
{
     Task<ResutTypeDto<RoomType>> GetRoomTypes(Guid id);

     Task<ResultDto> UpdateRoomType(UpdateRoomTypeDto data, Guid id);
     Task<ResultDto> AddRoomType(RoomTypeDto? data);
     Task<ResultDto> RemoveRoomType(Guid id);
}

