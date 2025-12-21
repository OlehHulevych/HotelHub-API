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

        
        var RoomType = await _context.RoomTypes.FirstOrDefaultAsync(type => type.Name == data.RoomType);

        var newRoom = new Room
        {
            Name = data.Name,
            PricePerNight = data.pricePerNight,
            RoomTypeId = RoomType.Id,
            Description = data.Description,
            Photos = new List<Photo>()
            
            
        };
        await _context.Rooms.AddAsync(newRoom);
        
        foreach (var photo in data.Photos)
        {
            if (photo.Length > 0)
            {
                using var stream = photo.OpenReadStream();
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(photo.FileName, stream),
                    Folder = "HotelHub"

                };
                var UploadResult = new ImageUploadResult();
                UploadResult = await _cloudinary.UploadAsync(uploadParams);
                Console.WriteLine("_______________________");
                Console.WriteLine("This is: "+UploadResult.SecureUrl);
                Console.WriteLine("This is: "+UploadResult.PublicId);
                Console.WriteLine("_______________________");
                //Console.WriteLine("This is Error: "+UploadResult.Error.Message);
                var photoInfo = new Photo
                {
                    Uri = UploadResult.SecureUrl.AbsoluteUri,
                    public_id = UploadResult.PublicId,
                    Room = newRoom
                };
                newRoom.Photos.Add(photoInfo);
                



            }
        }
        await _context.SaveChangesAsync();
        
        return new ResultDTO
        {
            result = true,
            Message = "The room is created",
            
            
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

    public async Task<ResultDTO> updateRoom(UpdateRoomDTO data, Guid id)
    {
        var roomForUpdate = await _context.Rooms.Include(r=>r.Type).Include(r=>r.Photos).FirstOrDefaultAsync(r=>r.Id == id);
        if (roomForUpdate == null)
        {
            return new ResultDTO
            {
                result = false,
                Message = "The room is not found"
            };
        }

        roomForUpdate.Name = String.IsNullOrEmpty(data.Name) ? roomForUpdate.Name:data.Name;
        roomForUpdate.Description = data.Description.Trim()==""? roomForUpdate.Description:data.Description;
        roomForUpdate.PricePerNight = data.pricePerNight == 0 ? roomForUpdate.PricePerNight:data.pricePerNight;
        if (!string.IsNullOrEmpty(data.RoomType))
        {
            var type = await _context.RoomTypes.FirstOrDefaultAsync(t => t.Name == data.RoomType);
            roomForUpdate.RoomTypeId = type.Id;
            roomForUpdate.Type = type;
        }

        if (data.deletedPhotos.Any())
        {
            foreach (var public_id in data.deletedPhotos)
            {
                var deletionParams = new DeletionParams(public_id);
                await _cloudinary.DestroyAsync(deletionParams);
                var photo = await _context.Photos.FirstOrDefaultAsync(photo => photo.public_id == public_id);
                roomForUpdate.Photos.Remove(photo);
                _context.Photos.Remove(photo);
            }
        }

        if (data.newPhotos.Any())
        {
            foreach (var photo in data.newPhotos)
            {
                if(photo.Length>0)
                {
                    using var newStream = photo.OpenReadStream();
                    var UploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(photo.FileName, newStream),
                        Folder = "HotelHub/" + roomForUpdate.Name
                    };
                    var UploadResult = new ImageUploadResult();
                    UploadResult = await _cloudinary.UploadAsync(UploadParams);
                    var photoInfo = new Photo
                    {
                        public_id = UploadResult.PublicId,
                        Uri = UploadResult.SecureUrl.AbsoluteUri,
                        Room = roomForUpdate
                    };
                    roomForUpdate.Photos.Add(photoInfo);
                }
                
               
            }
        }

        
        
        await _context.SaveChangesAsync();
        return new ResultDTO
        {
            Message = "The room is updated",
            Item = roomForUpdate,
            result = true


        };


    }

    public async Task<ResultDTO> deleteRoom(Guid id)
    {
        var room = await _context.Rooms.Include(r=>r.Photos).FirstOrDefaultAsync(r => r.Id == id);
        
        if (room == null)
        {
            return new ResultDTO()
            {
                result = false,
                Message = "The room is not found"
            };
        }
        foreach (var photo in room.Photos)
        {
            var deleteParams = new DeletionParams(photo.public_id);
            await _cloudinary.DestroyAsync(deleteParams);
        }
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
            query = _context.Rooms.Include(r => r.Type).Include(r=>r.Photos).Where(r=>r.Type.Name == pagination.type).AsQueryable();
        }
        else
        {
            query = _context.Rooms.Include(r => r.Type).Include(r=>r.Photos).AsQueryable();
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