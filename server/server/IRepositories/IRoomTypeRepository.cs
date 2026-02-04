using server.DTO;
using server.models;
using server.ResponseDTO;

namespace server.IRepositories;

public interface IRoomTypeRepository
{
     Task<ResutTypeDto<RoomTypeDTO>> GetRoomTypes(Guid id);

     Task<ResultDto> UpdateRoomType(UpdateRoomTypeDto data, Guid id);
     Task<ResultDto> AddRoomType(RoomTypeDto? data);
     Task<ResultDto> RemoveRoomType(Guid id);
}

