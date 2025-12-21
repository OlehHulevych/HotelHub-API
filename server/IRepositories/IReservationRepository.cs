using server.DTO;
using server.models;

namespace server.IRepositories;

public interface IReservationRepository
{
    public Task<PaginatedItemsDTO<Reservation>> getAllReservation(PaginationDTO query);
    public Task<PaginatedItemsDTO<Reservation>> getAllReservationById(PaginationDTO query, string id);
    public Task<ResultDTO> getOneReservation(Guid id);
    public Task<ResultDTO> createReservation(ReservationDTO data, string id);
    public Task<ResultDTO> editReservation(UpdateReservationDTO data, Guid id);
    public Task<ResultDTO> deleteReservation(Guid id, string userId);

}