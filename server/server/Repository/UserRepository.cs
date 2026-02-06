
using server.DTO;
using server.IRepositories;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using server.Data;
using server.Helpers;
using server.models;
using server.ResponseDTO;
using server.Tools;


namespace server.Repository;

public class UserRepository:IUserRepository
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> _userManager;
    private readonly JwtTokenService _jwtTokenService;
    private readonly Cloudinary _cloudinary;

    public UserRepository(ApplicationDbContext context, UserManager<User> userManager, JwtTokenService jwtTokenService, IOptions<CloudinarySettings> config)
    {
        _context = context;
        _userManager = userManager;
        _jwtTokenService = jwtTokenService;
        var acc = new Account(config.Value.CloudName, config.Value.ApiKey, config.Value.ApiSecret);
        _cloudinary = new Cloudinary(acc);
    }
    public async Task<ResultDto> RegisterUser(RegisterDto data)
    {
        if (string.IsNullOrEmpty(data.Name) && string.IsNullOrEmpty(data.Password))
        {
            return new ResultDto
            {
                Result = false,
                Message = "There is no information for register User"
            };
        }

        User user = new User
        {
            Name = data.Name,
            Email = data.Email,
            UserName = data.Email, 
            Position = data.Position,
            Reservations = new List<Reservation>(),
            Role = Roles.User
            
        };
        var result = await _userManager.CreateAsync(user, data.Password);
        Console.WriteLine(result);
        if (!result.Succeeded)
        {
            var errors = result.Errors.ToList();
            return new ResultDto
            {
                Result = false,
                Message = errors[0].Description
            };
        }

        if (data.Avatar?.Length > 0)
        {
            await using var stream = data.Avatar.OpenReadStream();
            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(data.Avatar.FileName, stream),
                Folder = "HotelHub/avatars"
            };
            var uploadResult = await _cloudinary.UploadAsync(uploadParams);
            var avatarPhoto = new AvatarUser
            {
                AvatarPath = uploadResult.SecureUrl.AbsoluteUri,
                UserId = user.Id,
                PublicId = uploadResult.PublicId,
                User = user
            };

            await _context.AvatarUsers.AddAsync(avatarPhoto);
        }
        
        await _context.SaveChangesAsync();
        
        
        if (data.Role == "ADMIN" && String.IsNullOrEmpty(data.Position))
        {
            await _userManager.AddToRoleAsync(user, Roles.Admin);
        }

        if (data.Role == "OWNER")
        {
            await _userManager.AddToRoleAsync(user, Roles.Owner);
        }

        if (data.Role == "STAFF")
        {
            await _userManager.AddToRoleAsync(user, Roles.Staff);
        }

        await _userManager.AddToRoleAsync(user, Roles.User);

        UserDto userDto = new UserDto
        {
            Name = user.Name,
            Email = user.Email,
            
        };

        return new ResultDto
        {
            Result = true,
            Message = "The user is registered",
            Item = userDto
        };



    }

    public async Task<LoginResponseDto> LoginUser(LoginDto data)
    {
        if (string.IsNullOrWhiteSpace(data.Email) && string.IsNullOrWhiteSpace(data.Password))
        {
            return new LoginResponseDto
            {
                Error = "Email and password are required."
            };
           
        }

        var foundUser = await _userManager.FindByEmailAsync(data.Email);
       
        if (string.IsNullOrEmpty(foundUser?.Id))
        {
            return new LoginResponseDto
            {
                Error = "The user is not found"
            };
        }

        await _userManager.GetRolesAsync(foundUser);
        var token = await _jwtTokenService.CreateToken(foundUser);
        if (string.IsNullOrEmpty(token))
        {
            return new LoginResponseDto
            {
                Error = "Something went wrong during creating token"
            };
        }

        var validPassword = await _userManager.CheckPasswordAsync(foundUser, data.Password);
        if (!validPassword)
        {
            return new LoginResponseDto
            {
                Error = "Password is invalid"
            };
        }

        return new LoginResponseDto
        {
            FoundUser = foundUser,
            Token = token
        };

    }

    public async Task<ResultDto> EditUser(EditUserDtO data, string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return new ResultDto
            {
                Result = false,
                Message = "There is no id of User"
            };
        }
        if (data.Email == null && data.Name == null && data.PhotoFile == null)
        {
            return new ResultDto
            {
                Result = false,
                Message = "The data is empty for update user information"
            };
        }

        User? updatedUser = await _userManager.Users.Include(u=>u.AvatarUser).FirstOrDefaultAsync(u=>u.Id==id);
        if (updatedUser == null)
        {
            return new ResultDto
            {
                Result = false,
                Message = "The user is not found"
            };
        }

        if (data.PhotoFile is { Length: > 0 })
        {
            await using var stream = data.PhotoFile.OpenReadStream();
            ImageUploadParams imageUploadParams = new ImageUploadParams
            {
                File = new FileDescription(data.PhotoFile.FileName, stream),
                Folder = "HotelHub/avatars"
            };
            var imageUploadResult = await _cloudinary.UploadAsync(imageUploadParams);
            AvatarUser avatar = updatedUser.AvatarUser;
            var deletionParams = new DeletionParams(avatar.PublicId);
            await _cloudinary.DestroyAsync(deletionParams);
            avatar.PublicId = imageUploadResult.PublicId;
            avatar.AvatarPath = imageUploadResult.SecureUrl.AbsoluteUri;
        }

        updatedUser.Name = data.Name ?? updatedUser.Name;
        updatedUser.Email = data.Email ?? updatedUser.Email;
        await _context.SaveChangesAsync();
        return new ResultDto
        {
            Result = true,
            Message = "The user information is update",
            Item = updatedUser
        };
    }


    public async Task<ResultDto> ChangeUserPassword(string id, ChnagePasswordDto model)
    {
        Console.WriteLine("Hello world");
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return new ResultDto
            {
                Result = false,
                Message = "The user is not found"
            };
        }

        
        

        var setNewPassword = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
        if (!setNewPassword.Succeeded)
        {
            return new ResultDto()
            {
                Result = false,
                Message = "Invalid Password",
            };
        }

        return new ResultDto()
        {
            Result = true,
            Message = "Password was changed"
        };


    }
    
    public async Task<UserResultDto> GetUserInformation(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return new UserResultDto
            {
                Result = false,
                Message = "There is no id ",

            };
        }

        var foundUser = await _userManager.Users.Include(u => u.AvatarUser).FirstOrDefaultAsync(user => user.Id == id);
        if (foundUser == null)
        {
            return new UserResultDto
            {
                Result = false,
                Message = "The user is not found"
            };
        }

        var roles = await _userManager.GetRolesAsync(foundUser);
        var avatar = await _context.AvatarUsers.FirstOrDefaultAsync(a=>a.UserId==foundUser.Id);

        UserDTO user = new UserDTO
        (
            foundUser.Id,
            foundUser.Name,
            foundUser.Email,
            foundUser.Position,
            foundUser.OnDuty,
            !String.IsNullOrEmpty(avatar.AvatarPath)?foundUser.AvatarUser.AvatarPath:null,
            foundUser.Banned
        );

        return new UserResultDto
        {
            Result = true,
            Message = "Welcome",
            Item = user,
            roles = roles
        };
    }

    public async Task<ResultDto> DeleteUser(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return new ResultDto
            {
                Result = false,
                Message = "There is no any id for deleting user"
            };
        }

        var user = await _userManager.FindByIdAsync(id);
        if (user==null)
        {
            return new ResultDto
            {
                Result = false,
                Message = "The user is not found"
            };
        }
        var result = await _userManager.DeleteAsync(user);
        if (!result.Succeeded)
        {
            return new ResultDto
            {
                Message = "Something went wrong during deletion",
                Result = false
            };
        }
        return new ResultDto()
        {
            Message = "The user was deleted",
            Result = true
        };

    }

    public async Task<ResultDto> PromoteUser(string id, PromoteDTO data)
    {
        var user = await _userManager.FindByIdAsync(id);
        
        if (user == null)
        {
            return new ResultDto()
            {
                Message = "The user is not found",
                Result = false
            };
        }

       
        await _userManager.AddToRoleAsync(user, Roles.Admin);
        user.Position = data.Position;
        user.OnDuty = true;
        await _context.SaveChangesAsync();
        return new ResultDto
        {
            Result = true,
            Message = "The user is promoted"
        };
        

    }

    public async Task<ResultDto> BanUser(string id)
    {
        if (String.IsNullOrEmpty(id))
        {
            return new ResultDto
            {
                Result = false,
                Message = "There is no user Id"

            };
        }

        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return new ResultDto
            {
                Result = false,
                Message = "The User is not found"
            };
        }

        if (user.Banned)
        {
            user.Banned = false;
            user.OnDuty = true;
            await _context.SaveChangesAsync();
            return new ResultDto
            {
                Result = true,
                Message = "The user is unbanned"
            };
            
        }
        user.Banned = true;
        user.OnDuty = false;
        await _context.SaveChangesAsync();
        return new ResultDto
        {
            Result = true,
            Message = "The user is banned"
        };
    }

    public async Task<PaginatedItemsDto<UserDTO>> GetAllStaff(int currentPage)
    {
        var query = from u in _context.Users.AsNoTracking()
            join a in _context.AvatarUsers on u.Id equals a.UserId
            join ur in _context.UserRoles on u.Id equals ur.UserId
            join r in _context.Roles on ur.RoleId equals r.Id
            where r.Name == "STAFF"
            select u;

        var length = await query.CountAsync();
        var users = await query.Distinct().Skip((currentPage - 1) * 10).Take(10).Select(u => new UserDTO
        (
            u.Id,
            u.Name,
            u.Email,
            u.Position,
            u.OnDuty,
            u.AvatarUser!=null && !String.IsNullOrEmpty(u.AvatarUser.AvatarPath)?u.AvatarUser.AvatarPath:null,
            u.Banned
        )).ToListAsync();

        return new PaginatedItemsDto<UserDTO>
        {
            Items = users,
            TotalLength = length,
            CurrentPage = currentPage,
            TotalPage = (int)Math.Ceiling((double)length / 10)
        };
    }
}