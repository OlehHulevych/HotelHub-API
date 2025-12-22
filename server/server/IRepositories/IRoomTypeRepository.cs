using server.DTO;
using server.models;

namespace server.IRepositories;

public interface IRoomTypeRepository
{
     Task<ResutTypeDto<RoomType>> getRoomTypes(Guid Id);

     Task<ResultDTO> UpdateRoomType(UpdateRoomTypeDTO data, Guid id);
     Task<ResultDTO> AddRoomType(RoomTypeDTO? data);
     Task<ResultDTO> RemoveRoomType(Guid id);
}

