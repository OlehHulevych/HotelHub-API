using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using server.Data;
using server.DTO;
using server.Helpers;
using server.IRepositories;
using server.models;

namespace server.Repository;

public class RoomTypeRepository:IRoomTypeRepository
{
    private ApplicationDbContext _context;
    private readonly Cloudinary _cloudinary;

    public RoomTypeRepository(ApplicationDbContext context, IOptions<CloudinarySettings> config)
    {
        _context = context;
        var acc = new Account(config.Value.CloudName, config.Value.ApiKey, config.Value.ApiSecret);
        _cloudinary = new Cloudinary(acc);
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
            Quantity = 0,
            PricePerNight = data.pricePerNight,
            Description = data.Description,
            Photos = new List<Photo>()
        };
        
        foreach (var photo in data.Photos)
        {
            if (photo.Length > 0)
            {
                using var stream = photo.OpenReadStream();
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(photo.FileName, stream),
                    Folder = "HotelHub/rooms"

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
                    RoomType = newRoomType
                };
                newRoomType.Photos.Add(photoInfo);
                



            }
        }

        var newDetail = new Detail
        {
            Capacity = data.Capacity,
            Norishment = data.Norishment,
            Spa = data.Spa,
            View = data.View,
            RoomTypeId = newRoomType.Id,
            RoomType = newRoomType
        };
        newRoomType.Detail = newDetail;
        await _context.RoomTypes.AddAsync(newRoomType);
        await _context.SaveChangesAsync();
        return new ResultDTO()
        {
            result = true,
            Message = "The room type was added",
            Item = newRoomType
        };


    }

    public async Task<ResultDTO> UpdateRoomType(UpdateRoomTypeDTO data, Guid Id)
    {
        var roomTypeForUpdate = await _context.RoomTypes.Include(rt=>rt.Photos).Include(rt=>rt.Detail).FirstOrDefaultAsync(rt => rt.Id == Id);
        if (data.deletedPhotos.Any())
        {
            foreach (var public_id in data.deletedPhotos)
            {
                var deletionParams = new DeletionParams(public_id);
                await _cloudinary.DestroyAsync(deletionParams);
                var photo = await _context.Photos.FirstOrDefaultAsync(photo => photo.public_id == public_id);
                roomTypeForUpdate.Photos.Remove(photo);
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
                        Folder = "HotelHub/rooms" + roomTypeForUpdate.Name
                    };
                    var UploadResult = new ImageUploadResult();
                    UploadResult = await _cloudinary.UploadAsync(UploadParams);
                    var photoInfo = new Photo
                    {
                        public_id = UploadResult.PublicId,
                        Uri = UploadResult.SecureUrl.AbsoluteUri,
                        RoomType = roomTypeForUpdate
                    };
                    roomTypeForUpdate.Photos.Add(photoInfo);
                }
                
               
            }
        }
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