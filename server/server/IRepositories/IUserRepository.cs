using server.DTO;
using server.models;

namespace server.IRepositories;

public interface IUserRepository
{
    Task<ResultDTO> RegisterUser(RegisterDTO data);
    Task<LoginResponseDTO> LoginUser(LoginDTO data);
    Task<ResultDTO> ChangeUserPassword(string id, ChnagePasswordDTO model);
    Task<ResultDTO> getUserInformation(string id);
    Task<ResultDTO> deleteUser(string id);
    Task<ResultDTO> promoteUser(string id);
    Task<PaginatedItemsDTO<User>> getAllUser(int currentpage);
}