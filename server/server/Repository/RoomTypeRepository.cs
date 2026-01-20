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
    public async Task<ResultDto> AddRoomType(RoomTypeDto? data)
    {
        if (data == null)
        {
            return new ResultDto()
            {
                Result = false,
                Message = "There is no name of new type"
            };
        }

        var newRoomType = new RoomType
        {
            Name = data.Name,
            RoomList = new List<Room>(),
            Quantity = 0,
            PricePerNight = data.PricePerNight,
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
                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                Console.WriteLine("_______________________");
                Console.WriteLine("This is: "+uploadResult.SecureUrl);
                Console.WriteLine("This is: "+uploadResult.PublicId);
                Console.WriteLine("_______________________");
                //Console.WriteLine("This is Error: "+UploadResult.Error.Message);
                var photoInfo = new Photo
                {
                    Uri = uploadResult.SecureUrl.AbsoluteUri,
                    PublicId = uploadResult.PublicId,
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
        return new ResultDto()
        {
            Result = true,
            Message = "The room type was added",
            Item = newRoomType
        };


    }

    public async Task<ResutTypeDto<RoomType>> GetRoomTypes(Guid id)
    {
        List<RoomType> items;
        RoomType? item;
        if (id == Guid.Empty)
        {
            items = await _context.RoomTypes.Include(t=>t.RoomList).Include(rt=>rt.Photos).Include(rt=>rt.Detail).ToListAsync();
            if (items.Count <= 0)
            {
                return new ResutTypeDto<RoomType>
                {
                    Result = false,
                    Message = "The types are not found"
                };
            }

            return new ResutTypeDto<RoomType>
            {
                Result = true,
                Message = "The types are retrived",
                Items = items
            };
            
        }

        item = await _context.RoomTypes.Include(rt=>rt.Photos).Include(rt=>rt.Photos).Include(rt=>rt.Detail).FirstOrDefaultAsync(i => id.Equals(i.Id));
        if (item == null)
        {
            return new ResutTypeDto<RoomType>
            {
                Result = false,
                Message = "The type is not found",

            };
        }

        return new ResutTypeDto<RoomType>
        {
            Result = true,
            Message = "The item is retrived",
            Item = item
        };



    }

    public async Task<ResultDto> UpdateRoomType(UpdateRoomTypeDto data, Guid id)
    {
        var roomTypeForUpdate = await _context.RoomTypes.Include(rt => rt.Photos).Include(rt => rt.Detail)
            .FirstOrDefaultAsync(rt => rt.Id == id);
        if (roomTypeForUpdate == null)
        {
            return new ResultDto
            {
                Result = false,
                Message = "The room type is not found",

            };
        }
        try
        {
           

            if (data.DeletedPhotos.Any())
            {
                foreach (var publicId in data.DeletedPhotos)
                {
                    var deletionParams = new DeletionParams(publicId);
                    await _cloudinary.DestroyAsync(deletionParams);
                    var photo = await _context.Photos.FirstOrDefaultAsync(photo => photo.PublicId == publicId);
                    if (photo != null)
                    {
                        roomTypeForUpdate.Photos.Remove(photo);
                        _context.Photos.Remove(photo);
                    }
                }
            }

            if (data.NewPhotos.Any())
            {
                foreach (var photo in data.NewPhotos)
                {
                    if (photo.Length > 0)
                    {
                        using var newStream = photo.OpenReadStream();
                        var uploadParams = new ImageUploadParams()
                        {
                            File = new FileDescription(photo.FileName, newStream),
                            Folder = "HotelHub/rooms"
                        };
                        var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                        var photoInfo = new Photo
                        {
                            PublicId = uploadResult.PublicId,
                            Uri = uploadResult.SecureUrl.AbsoluteUri,
                            RoomTypeId = roomTypeForUpdate.Id,
                            RoomType = roomTypeForUpdate
                        };
                        roomTypeForUpdate.Photos.Add(photoInfo);
                        await _context.Photos.AddAsync(photoInfo);
                    }


                }
            }

            roomTypeForUpdate.Name = !String.IsNullOrEmpty(data.Name) ? data.Name : roomTypeForUpdate.Name;
            roomTypeForUpdate.Description = !String.IsNullOrEmpty(data.Description)
                ? data.Description
                : roomTypeForUpdate.Description;
            roomTypeForUpdate.PricePerNight =
                data.PricePerNight != 0 ? data.PricePerNight : roomTypeForUpdate.PricePerNight;
            var typeDetail = roomTypeForUpdate.Detail;
            typeDetail.Capacity = data.Capacity != 0 ? data.Capacity : typeDetail.Capacity;
            typeDetail.Norishment = data.Norishment.Count > 0 ? data.Norishment : typeDetail.Norishment;
            typeDetail.Spa = data.Spa.Count > 0 ? data.Spa : typeDetail.Spa;
            typeDetail.View = data.Spa.Count > 0 ? data.View : typeDetail.View;


            await _context.SaveChangesAsync();
            return new ResultDto
            {
                Result = true,
                Message = "The room type was updated",
                Item = roomTypeForUpdate
            };


        }
        catch (DbUpdateConcurrencyException e)
        {
            var entry = e.Entries.Single();
            var databaseValues = await entry.GetDatabaseValuesAsync();
            if (databaseValues == null)
            {
                return new ResultDto
                {
                    Result = false,
                    Message = "The room type was deleted by another user."
                };
            }
            entry.OriginalValues.SetValues(databaseValues);
            await _context.SaveChangesAsync();
            return new ResultDto { Result = true, Message = "The room type was updated", Item = roomTypeForUpdate };
        }
        
    }

    public async Task<ResultDto> RemoveRoomType(Guid id)
    {
        if (id == null)
        {
            return new ResultDto
            {
                Result = false,
                Message = "There is no any id"
            };
        }

        var foundType = await _context.RoomTypes.FirstOrDefaultAsync(type => type.Id == id);
        if (foundType == null)
        {
            return new ResultDto
            {
                Result = false,
                Message = "The room type is not found",
            };
        }
        _context.RoomTypes.Remove(foundType);
        await _context.SaveChangesAsync();
        return new ResultDto
        {
            Result = false,
            Message = "The room type was deleted"
        };
        

    }
    
}