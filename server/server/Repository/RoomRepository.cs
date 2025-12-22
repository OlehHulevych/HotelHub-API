using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using server.Data;
using server.DTO;
using server.Helpers;
using server.IRepositories;
using server.models;
using server.Tools;

namespace server.Repository;

public class RoomRepository:IRoomRepository
{
    private readonly ApplicationDbContext _context;
    private readonly Cloudinary _cloudinary;
    

    public RoomRepository(ApplicationDbContext context,  IOptions<CloudinarySettings> config)
    {
        _context = context;
        var acc = new Account(config.Value.CloudName, config.Value.ApiKey, config.Value.ApiSecret);
        _cloudinary = new Cloudinary(acc);
        

    }
    public async Task<ResultDTO> createRoom(RoomDTO data)
    {
        if (data == null)
        {
            return new ResultDTO
            {
                result = false,
                Message = "There is no data"
            };
        }

        
        var roomType = await _context.RoomTypes.FirstOrDefaultAsync(type => type.Name == data.RoomType);
        roomType!.Quantity += 1;
        var newRoom = new Room
        {
            Name = data.Name,
            RoomTypeId = roomType.Id,
            Type = roomType,
            Number = data.Number
            
            
        };
        await _context.Rooms.AddAsync(newRoom);
        
        await _context.SaveChangesAsync();
        
        return new ResultDTO
        {
            result = true,
            Message = "The room is created",
            Item = newRoom
            
            
        };


    }

    public async Task<ResultDTO> getRoom(Guid id)
    {
        var Room = await _context.Rooms.FirstOrDefaultAsync(r => r.Id == id);
        if (Room == null)
        {
            return new ResultDTO
            {
                result = false,
                Message = "The room is not found"
            };
        }

        return new ResultDTO
        {
            result = true,
            Message = "The Room is found",
            Item = Room
        };

    }
    

    public async Task<ResultDTO> deleteRoom(Guid id)
    {
        var room = await _context.Rooms.Include(r=>r.Type).FirstOrDefaultAsync(r => r.Id == id);
        
        if (room == null)
        {
            return new ResultDTO()
            {
                result = false,
                Message = "The room is not found"
            };
        }

        room.Type.Quantity -= 1;
        _context.Rooms.Remove(room);
        await _context.SaveChangesAsync();
        return new ResultDTO
        {
            result = true,
            Message = "The room was deleted"
        };
    }

    public async Task<PaginatedItemsDTO<Room>> getALlRooms(PaginationDTO pagination)
    {
        IQueryable<Room> query;
        
        if (pagination.type != "")
        {
            query = _context.Rooms.Include(r => r.Type).Where(r=>r.Type.Name == pagination.type).AsQueryable();
        }
        else
        {
            query = _context.Rooms.Include(r => r.Type).AsQueryable();
        }
        
        var length = await _context.Rooms.CountAsync();
        var items = await query.OrderBy(p => p.Id)
            .Skip((pagination.currentPage - 1) * 10)
            .Take(10)
            .ToListAsync();
        if (items == null)
        {
            return null;
        }

        return new PaginatedItemsDTO<Room>()
        {
            Items = items,
            CurrentPage = pagination.currentPage,
            TotalPage = length / 10
        };


    }
}