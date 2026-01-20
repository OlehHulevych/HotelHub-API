
using Microsoft.EntityFrameworkCore;
using server.Data;
using server.DTO;
using server.IRepositories;
using server.models;


namespace server.Repository;

public class RoomRepository:IRoomRepository
{
    private readonly ApplicationDbContext _context;
    

    public RoomRepository(ApplicationDbContext context)
    {
        _context = context;
        
        

    }
    public async Task<ResultDto> CreateRoom(RoomDto data)
    {
        var roomType = await _context.RoomTypes.FirstOrDefaultAsync(type => type.Id == data.RoomTypeId);
        roomType!.Quantity += 1;
        var newRoom = new Room
        {
            RoomTypeId = roomType.Id,
            Type = roomType,
            Number = data.Number
            
            
        };
        await _context.Rooms.AddAsync(newRoom);
        
        await _context.SaveChangesAsync();
        
        return new ResultDto
        {
            Result = true,
            Message = "The room is created",
            Item = newRoom
            
            
        };


    }

    
    

    public async Task<ResultDto> DeleteRoom(Guid id)
    {
        var room = await _context.Rooms.Include(r=>r.Type).FirstOrDefaultAsync(r => r.Id == id);
        
        if (room == null)
        {
            return new ResultDto()
            {
                Result = false,
                Message = "The room is not found"
            };
        }

        room.Type.Quantity -= 1;
        _context.Rooms.Remove(room);
        await _context.SaveChangesAsync();
        return new ResultDto
        {
            Result = true,
            Message = "The room was deleted"
        };
    }

    public async Task<PaginatedItemsDto<Room>> GetALlRooms(PaginationDto pagination)
    {
        IQueryable<Room> query;
        
        if (pagination.TypeId!=Guid.Empty)
        {
            query = _context.Rooms.Include(r => r.Type).Where(r=>r.Type.Id == pagination.TypeId).AsQueryable();
        }
        else
        {
            query = _context.Rooms.Include(r => r.Type).AsQueryable();
        }
        
        var length = await _context.Rooms.CountAsync();
        var items = await query.OrderBy(p => p.Id)
            .Skip((pagination.CurrentPage - 1) * 10)
            .Take(10)
            .ToListAsync();
        if (!items.Any())
        {
            return null;
        }

        return new PaginatedItemsDto<Room>()
        {
            Items = items,
            CurrentPage = pagination.CurrentPage,
            TotalPage = length / 10
        };


    }
}