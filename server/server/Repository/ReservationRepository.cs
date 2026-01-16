using System.Runtime.InteropServices.JavaScript;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using server.Data;
using server.DTO;
using server.IRepositories;
using server.models;

namespace server.Repository;

public class ReservationRepository:IReservationRepository
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> _userManager;

    public ReservationRepository(ApplicationDbContext context,UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<PaginatedItemsDTO<Reservation>> getAllReservation(PaginationDTO query)
    {
        IQueryable<Reservation> queries = _context.Reservations.Include(r=>r.User).Include(r=>r.Room).OrderByDescending(r=>r.Id).AsQueryable();
        var length = await _context.Reservations.CountAsync();
        var items = await queries.Skip((query.currentPage - 1) * 10).Take(10).ToListAsync();
        return new PaginatedItemsDTO<Reservation>
        {
            Items = items,
            CurrentPage = query.currentPage,
            TotalPage = length / 10

        };
    }

    public async Task<PaginatedItemsDTO<Reservation>> getAllReservationById(PaginationDTO query, string id)
    {
        IQueryable<Reservation> queries = _context.Reservations.Where(r=>r.UserId==id).Include(r=>r.Room).ThenInclude(r=>r.Type).ThenInclude(t=>t.Photos).OrderByDescending(r=>r.Id).AsQueryable();
        var length = await _context.Reservations.CountAsync();
        var items = await queries.Skip((query.currentPage - 1) * 10).Take(10).ToListAsync();
        return new PaginatedItemsDTO<Reservation>
        {
            Items = items,
            CurrentPage = query.currentPage,
            TotalPage = length / 10

        };
    }

    public async Task<ResultDTO> getOneReservation(Guid id)
    {
        var reservation = await _context.Reservations.Include(r=>r.Room).FirstOrDefaultAsync(r => r.Id == id);
        if (reservation == null)
        {
            return new ResultDTO
            {
                result = false,
                Message = "The reservation is not found",

            };
        }

        return new ResultDTO
        {
            result = true,
            Message = "Your reservation is found",
            Item = reservation
        };
    }

    public async Task<ResultReservationDTO> createReservation(ReservationDTO data, string id)
    {
        var userCheckOut = data.CheckOut;
        var userCheckIn = data.CheckIn;
        if (userCheckIn < DateOnly.FromDateTime(DateTime.Now) ||
            userCheckOut < DateOnly.FromDateTime(DateTime.Now))
        {
            return new ResultReservationDTO()
            {
                Message = "The period which is choose is past.Please select another",
                result = false
            };
        }

        var type = await _context.RoomTypes.FirstOrDefaultAsync(rt => rt.Id == data.TypeId);
        var typeRooms = await _context.Rooms.Include(r=>r.Reservations).Where(r=>r.RoomTypeId==data.TypeId).ToListAsync();
        var availableRoom = typeRooms.FirstOrDefault(room =>
            !room.Reservations.Any(res => res.CheckInDate < userCheckOut && res.CheckOutDate > userCheckIn));
        
        Room? reservedRoom = null;
        var user = await _context.Users.FirstOrDefaultAsync(u=>u.Id==id);
        if (availableRoom == null)
        {
            return new ResultReservationDTO
            {
                result = false,
                Message = "There is no room at this date"
            };
        }

        var days = userCheckOut.DayNumber - userCheckIn.DayNumber;
        
        var totalPrice = type.PricePerNight * days;
        

        Reservation userReservation = new Reservation
        {
            CheckInDate = userCheckIn,
            CheckOutDate = userCheckOut,
            UserId = id,
            User = user,
            RoomId = availableRoom.Id,
            Room = availableRoom,
            Status = Statuses.Active,
            TotalPrice = totalPrice

        };
        
        await _context.Reservations.AddAsync(userReservation);
        await _context.SaveChangesAsync();
        return new ResultReservationDTO
        {
            result = true,
            Message = "The reservation was created",
            Item = userReservation,
            reservedRoom = reservedRoom
            
        };
    }

    public async Task<ResultDTO> editReservation(UpdateReservationDTO data, Guid id)
    {
        var userReservation = await _context.Reservations.Include(reserv=>reserv.Room).ThenInclude(r=>r.Type).FirstOrDefaultAsync(r=>r.Id==id);
        var reservedRoom = await _context.Rooms.Include(r => r.Reservations).FirstOrDefaultAsync(r=>r.Id==userReservation.RoomId);
        var RoomReservations = reservedRoom.Reservations;
        var newCheckIn = data.CheckIn == DateOnly.MinValue ? userReservation.CheckInDate:data.CheckIn;
        var newCheckOut = data.CheckOut == DateOnly.MinValue ? userReservation.CheckInDate:data.CheckOut;

        foreach (var reservation in RoomReservations)
        {
            var checkIn = reservation.CheckInDate;
            var checkOut = reservation.CheckOutDate;
            if (checkOut > newCheckIn && reservation.Id != id || newCheckOut < checkOut && reservation.Id != id)
            {
                return new ResultDTO
                {
                    Message = "The range which you selected is occupied",
                    result = false
                };
            }
        }

        userReservation.CheckInDate = newCheckIn;
        userReservation.CheckOutDate = newCheckOut;
        userReservation.TotalPrice = (newCheckOut.DayNumber - newCheckIn.DayNumber) * reservedRoom.Type.PricePerNight;

        await _context.SaveChangesAsync();
        return new ResultDTO()
        {
            result = true,
            Message = "The reservation was updated"
        };
    }

    public async Task<ResultDTO> deleteReservation(Guid id, string userId)
    {
        if (id == Guid.Empty)
        {
            return new ResultDTO
            {
                Message = "There is no id of reservation",
                result = false
            };
        }

        var reservation = await _context.Reservations.Include(u=>u.User).FirstOrDefaultAsync(r => r.Id == id);
        if (reservation == null)
        {
            return new ResultDTO
            {
                Message = "The reservation is not found",
                result = false
            };
        }

        var checkingUser = await _userManager.FindByIdAsync(userId);
        var roles = await _userManager.GetRolesAsync(checkingUser);
        var isAdmin = false;
        foreach (var role in roles)
        {
            if (role == "ADMIN")
            {
                isAdmin = true;
            }
        }
        Console.WriteLine(isAdmin);
        if (!reservation.UserId.Equals(checkingUser.Id) && !isAdmin)
        {
            return new ResultDTO
            {
                Message = "Access is denied",
                result = false
            };
        }

        _context.Reservations.Remove(reservation);
        await _context.SaveChangesAsync();
        return new ResultDTO
        {
            result = true,
            Message = "The reservation was deleted"
        };
    }
}