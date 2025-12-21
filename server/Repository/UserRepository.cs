using System.ComponentModel;
using System.Runtime.InteropServices.JavaScript;
using server.DTO;
using server.IRepositories;
using BCrypt.Net;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using server.Data;
using server.Helpers;
using server.models;
using server.Tools;


namespace server.Repository;

public class UserRepository:IUserRepository
{
    private ApplicationDbContext _context;
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
    public async Task<ResultDTO> RegisterUser(RegisterDTO data)
    {
        if (data.Name == null && data.Password == null)
        {
            return null;
        }

        User user = new User
        {
            Name = data.Name,
            Email = data.Email,
            UserName = data.Email, 
            Reservations = new List<Reservation>(),
            Role = Roles.User
            
        };
        var result = await _userManager.CreateAsync(user, data.Password);
        Console.WriteLine(result);
        if (!result.Succeeded)
        {
            var errors = result.Errors.ToList();
            return new ResultDTO
            {
                result = false,
                Message = errors[0].Description
            };
        }

        if (data.Avatar?.Length > 0)
        {
            using var stream = data.Avatar.OpenReadStream();
            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(data.Avatar.FileName, stream),
                Folder = "HotelHub/avatars"
            };
            var UploadResult = new ImageUploadResult();
            UploadResult = await _cloudinary.UploadAsync(uploadParams);
            var AvatarPhoto = new AvatarUser
            {
                avatarPath = UploadResult.SecureUrl.AbsoluteUri,
                UserId = user.Id,
                User = user
            };

            await _context.AvatarUsers.AddAsync(AvatarPhoto);
        }
        
        await _context.SaveChangesAsync();
        
        
        if (data.Role == "ADMIN")
        {
            await _userManager.AddToRoleAsync(user, Roles.Admin);
        }

        await _userManager.AddToRoleAsync(user, Roles.User);

        UserDto userDto = new UserDto
        {
            Name = user.Name,
            Email = user.Email,
            
        };

        return new ResultDTO
        {
            result = true,
            Message = "The user is registered",
            Item = userDto
        };



    }

    public async Task<LoginResponseDTO> LoginUser(LoginDTO data)
    {
        if (string.IsNullOrWhiteSpace(data.Email) && string.IsNullOrWhiteSpace(data.Password))
        {
            return new LoginResponseDTO
            {
                Error = "Email and password are required."
            };
           ;
        }

        var foundUser = await _userManager.FindByEmailAsync(data.Email);
        foreach(PropertyDescriptor descriptor in TypeDescriptor.GetProperties(foundUser))
        {
            string name = descriptor.Name;
            object value = descriptor.GetValue(foundUser);
            Console.WriteLine("{0}={1}", name, value);
        }
        if (foundUser==null)
        {
            return new LoginResponseDTO
            {
                Error = "The user is not found"
            };
        }

        var roles = await _userManager.GetRolesAsync(foundUser);
        var token = await _jwtTokenService.CreateToken(foundUser);
        if (token == null)
        {
            return new LoginResponseDTO
            {
                Error = "Something went wrong during creating token"
            };
        }

        var validPassword = await _userManager.CheckPasswordAsync(foundUser, data.Password);
        if (!validPassword)
        {
            return new LoginResponseDTO
            {
                Error = "Password is invalid"
            };
        }

        return new LoginResponseDTO
        {
            FoundUser = foundUser,
            Token = token
        };

    }

    

    
    

    public async Task<ResultDTO> ChangeUserPassword(string id, ChnagePasswordDTO model)
    {
        Console.WriteLine("Hello world");
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return new ResultDTO
            {
                result = false,
                Message = "The user is not found"
            };
        }

        
        

        var setNewPassword = await _userManager.ChangePasswordAsync(user, model.oldPassword, model.newPassword);
        if (!setNewPassword.Succeeded)
        {
            return new ResultDTO()
            {
                result = false,
                Message = "Invalid Password",
            };
        }

        return new ResultDTO()
        {
            result = true,
            Message = "Password was changed"
        };


    }
    
    public async Task<ResultDTO> getUserInformation(string id)
    {
        if (id == null)
        {
            return new ResultDTO
            {
                result = false,
                Message = "There is no id ",

            };
        }

        var foundUser = await _userManager.Users.Include(u => u.AvatarUser).FirstOrDefaultAsync(user => user.Id == id);
        if (foundUser == null)
        {
            return new ResultDTO
            {
                result = false,
                Message = "The user is not found"
            };
        }

        return new ResultDTO
        {
            result = true,
            Message = "Welcome",
            Item = foundUser
        };
    }

    public async Task<ResultDTO> deleteUser(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return new ResultDTO
            {
                result = false,
                Message = "There is no any id for deleting user"
            };
        }

        var user = await _userManager.FindByIdAsync(id);
        var result = await _userManager.DeleteAsync(user);
        if (!result.Succeeded)
        {
            return new ResultDTO
            {
                Message = "Something went wrong during deletion",
                result = false
            };
        }
        return new ResultDTO()
        {
            Message = "The user was deleted",
            result = true
        };

    }

    public async Task<ResultDTO> promoteUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return new ResultDTO()
            {
                Message = "The user is not found",
                result = false
            };
        }

        await _userManager.AddToRoleAsync(user, Roles.Admin);
        return new ResultDTO
        {
            result = true,
            Message = "The user is promoted"
        };

    }

    public async Task<PaginatedItemsDTO<User>> getAllUser(int currentPage)
    {
        IQueryable<User> query =  _context.Users.AsQueryable();
        int length = await _context.Users.CountAsync();
        var users = await query.Skip((currentPage-1)*10).Take(10).ToListAsync();
        return new PaginatedItemsDTO<User>
        {
            Items = users,
            CurrentPage = currentPage,
            TotalPage = length / 10

        };
    }
}