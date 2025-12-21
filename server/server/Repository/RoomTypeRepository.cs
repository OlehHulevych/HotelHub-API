using Microsoft.EntityFrameworkCore;
using server.Data;
using server.DTO;
using server.IRepositories;
using server.models;

namespace server.Repository;

public class RoomTypeRepository:IRoomTypeRepository
{
    private ApplicationDbContext _context;

    public RoomTypeRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<ResultDTO> AddRoomType(RoomTypeDTO data)
    {
        if (data == null)
        {
            return new ResultDTO()
            {
                result = false,
                Message = "There is no name of new type"
            };
        }

        var newRoomType = new RoomType
        {
            Name = data.Name,
            RoomList = new List<Room>(),
            Quantity = 0
        };


        await _context.RoomTypes.AddAsync(newRoomType);
        await _context.SaveChangesAsync();
        return new ResultDTO()
        {
            result = true,
            Message = "The room type was added",
            Item = newRoomType
        };


    }

    public async Task<ResultDTO> RemoveRoomType(Guid id)
    {
        if (id == null)
        {
            return new ResultDTO
            {
                result = false,
                Message = "There is no any id"
            };
        }

        var foundType = await _context.RoomTypes.FirstOrDefaultAsync(type => type.Id == id);
        if (foundType == null)
        {
            return new ResultDTO
            {
                result = false,
                Message = "The room type is not found",
            };
        }
        _context.RoomTypes.Remove(foundType);
        await _context.SaveChangesAsync();
        return new ResultDTO
        {
            result = false,
            Message = "The room type was deleted"
        };
        

    }
    
}