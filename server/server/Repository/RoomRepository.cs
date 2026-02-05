
using Microsoft.EntityFrameworkCore;
using server.Data;
using server.DTO;
using server.IRepositories;
using server.models;
using server.ResponseDTO;


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
            Status = RoomStatus.Free,
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

    public async Task<ResultDto> PutInMaintenance(Guid id)
    {
        if (id==Guid.Empty)
        {
            return new ResultDto
            {
                Result = false,
                Message = "There is no room id"
            };
            
        }

        Room? room = await _context.Rooms.FirstOrDefaultAsync(r => r.Id==id);
        if (room == null)
        {
            return new ResultDto
            {
                Message = "The room is not found",
                Result = false,
                
            };
        }

        if (room.Status == RoomStatus.Occupied)
        {
            return new ResultDto
            {
                Result = true,
                Message = "This room is occupied"
            };
        }

        if (room.Status == RoomStatus.Free)
        {
            room.Status = RoomStatus.Maintenance;
        }
        else
        {
            room.Status = RoomStatus.Free;
        }
       

        await _context.SaveChangesAsync();
        return new ResultDto
        {
            Message = "The room is updated",
            Result = true,
            Item = room
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

        if (room.Status == RoomStatus.Occupied)
        {
            return new ResultDto
            {
                Result = true,
                Message = "This room cannot be deleted because it is occupied"
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

    public async Task<PaginatedItemsDto<RoomDTO>> GetALlRooms(PaginationDto pagination)
    {
        IQueryable<Room>  query = _context.Rooms.Where(r=> pagination.RoomStatus==RoomStatus.None || r.Status==pagination.RoomStatus).Include(r => r.Type).ThenInclude(t=>t.Photos).Include(r => r.Type).ThenInclude(t=>t.Detail).Where(r=>pagination.TypeId==Guid.Empty || r.RoomTypeId == pagination.TypeId).AsQueryable();
        
        var length = await _context.Rooms.CountAsync();
        var occupiedLength = await _context.Rooms.Where(r => r.Status == RoomStatus.Occupied).CountAsync();
        var maintenanceLength = await _context.Rooms.Where(r => r.Status == RoomStatus.Maintenance).CountAsync();
        var freeLength = await _context.Rooms.Where(r => r.Status == RoomStatus.Free).CountAsync();
        var items = await query.OrderBy(p => p.Id)
            .Skip((pagination.CurrentPage - 1) * 10)
            .Take(10).Select(r=> new RoomDTO(r.Id, r.Type.Name,r.Type.Detail.Capacity, r.Number,r.Type.PricePerNight,r.Type.Name,r.Status,r.Type.Photos.ToList()[0].Uri))
            .ToListAsync();
        if (!items.Any())
        {
            return null;
        }

        return new PaginatedItemsDto<RoomDTO>()
        {
            Items = items,
            CurrentPage = pagination.CurrentPage,
            TotalPage = (int) Math.Ceiling((double)length/10),
            TotalLength = length,
            OccupiedLength = occupiedLength,
            MaintenanceLength = maintenanceLength,
            FreeLength = freeLength
        };


    }
}