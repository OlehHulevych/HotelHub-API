using server.DTO;
using server.models;
using server.ResponseDTO;

namespace server.IRepositories;

public interface IUserRepository
{
    Task<ResultDto> RegisterUser(RegisterDto data);
    Task<LoginResponseDto> LoginUser(LoginDto data);
    Task<ResultDto> EditUser(EditUserDtO data, string id);
    Task<ResultDto> ChangeUserPassword(string id, ChnagePasswordDto model);
    Task<UserResultDto> GetUserInformation(string id);
    Task<ResultDto> DeleteUser(string id);
    Task<ResultDto> PromoteUser(string id, PromoteDTO data);
    Task<ResultDto> BanUser(string id);
    Task<PaginatedItemsDto<UserDTO>> GetAllStaff(int currentpage);
    
}