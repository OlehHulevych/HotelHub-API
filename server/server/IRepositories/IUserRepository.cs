using server.DTO;
using server.models;

namespace server.IRepositories;

public interface IUserRepository
{
    Task<ResultDto> RegisterUser(RegisterDto data);
    Task<LoginResponseDto> LoginUser(LoginDto data);
    Task<ResultDto> EditUser(EditUserDtO data, string id);
    Task<ResultDto> ChangeUserPassword(string id, ChnagePasswordDto model);
    Task<ResultDto> GetUserInformation(string id);
    Task<ResultDto> DeleteUser(string id);
    Task<ResultDto> PromoteUser(string id);
    Task<PaginatedItemsDto<User>> GetAllUser(int currentpage);
}