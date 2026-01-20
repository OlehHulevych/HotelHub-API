using server.DTO;
using server.models;

namespace server.IRepositories;

public interface IReservationRepository
{
    public Task<PaginatedItemsDto<Reservation>> GetAllReservation(PaginationDto query);
    public Task<PaginatedItemsDto<Reservation>> GetAllReservationById(PaginationDto query, string id);
    public Task<ResultDto> getOneReservation(Guid id);
    public Task<ResultReservationDto> CreateReservation(ReservationDto data, string id);
    public Task<ResultDto> EditReservation(UpdateReservationDto data, Guid id);
    public Task<ResultDto> DeleteReservation(Guid id, string userId);

}