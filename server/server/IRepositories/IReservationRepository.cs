using server.DTO;
using server.models;
using server.ResponseDTO;

namespace server.IRepositories;

public interface IReservationRepository
{
    public Task<PaginatedItemsDto<ReservationAdminDTO>> GetAllReservation(PaginationDto query);
    public Task<PaginatedItemsDto<ReservationDTO>> GetAllReservationById(PaginationDto query, string id);
    public Task<ResultDto> getOneReservation(Guid id);
    public Task<ResultReservationDto> CreateReservation(ReservationDto data, string id);
    public Task<ResultDto> EditReservation(UpdateReservationDto data, Guid id);
    public Task<ResultDto> DeleteReservation(Guid id, string userId);

}