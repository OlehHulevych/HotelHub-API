
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using server.Data;
using server.DTO;
using server.IRepositories;
using server.models;
using server.ResponseDTO;

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

    public async Task<PaginatedItemsDto<ReservationAdminDTO>> GetAllReservation(PaginationDto query)
    {
        IQueryable<Reservation> queries = _context.Reservations.Include(r=>r.User).Include(r=>r.Room).OrderByDescending(r=>r.Id).Where(reservation =>
            query.Status == null || reservation.Status == query.Status
        ).AsQueryable();
        var length = await _context.Reservations.CountAsync();
        var activeLength = await _context.Reservations.Where(reservation=>reservation.Status==Statuses.Active).CountAsync();
        var canceledLength = await _context.Reservations.Where(reservation => reservation.Status == Statuses.Canceled).CountAsync();
        var items = await queries.Skip((query.CurrentPage - 1) * 10).Take(10).Select(r=> new ReservationAdminDTO(r.Id, r.User.Name, r.Status, r.CheckInDate,r.CheckOutDate)).ToListAsync();
        return new PaginatedItemsDto<ReservationAdminDTO>
        {
            Items = items,
            CurrentPage = query.CurrentPage,
            TotalPage = (int)Math.Ceiling((double)length / 10),
            TotalLength = length,
            ActiveLength = activeLength,
            CanceledLength = canceledLength
        };
    }

    public async Task<PaginatedItemsDto<ReservationDTO>> GetAllReservationById(PaginationDto query, string id)
    {
        IQueryable<Reservation> queries = _context.Reservations.Where(r=>r.UserId==id).Include(r=>r.Room).ThenInclude(r=>r.Type).ThenInclude(t=>t.Photos).OrderByDescending(r=>r.Id).AsQueryable();
        var length = await _context.Reservations.CountAsync();
        var items = await queries.Skip((query.CurrentPage - 1) * 10).Take(10).Select(r=>new ReservationDTO(r.Id,r.Status,r.CheckInDate,r.CheckOutDate,r.Room.Type.Photos.Select(photo=>photo.Uri),r.Room.Type.Name )).ToListAsync();
        return new PaginatedItemsDto<ReservationDTO>
        {
            Items = items,
            CurrentPage = query.CurrentPage,
            TotalPage = length / 10

        };
    }

    public async Task<ResultDto> getOneReservation(Guid id)
    {
        var reservation = await _context.Reservations.Include(r=>r.Room).FirstOrDefaultAsync(r => r.Id == id);
        if (reservation == null)
        {
            return new ResultDto
            {
                Result = false,
                Message = "The reservation is not found",

            };
        }

        return new ResultDto
        {
            Result = true,
            Message = "Your reservation is found",
            Item = reservation
        };
    }

    public async Task<ResultReservationDto> CreateReservation(ReservationDto data, string id)
    {
        var userCheckOut = data.CheckOut;
        var userCheckIn = data.CheckIn;
        if (userCheckIn < DateOnly.FromDateTime(DateTime.Now) ||
            userCheckOut < DateOnly.FromDateTime(DateTime.Now))
        {
            return new ResultReservationDto()
            {
                Message = "The period which is choose is past.Please select another",
                Result = false
            };
        }

        var type = await _context.RoomTypes.FirstOrDefaultAsync(rt => rt.Id == data.TypeId);
        if (type == null)
        {
            return new ResultReservationDto
            {
                Result = false,
                Message = "The type is not found"
            };
        }
        var typeRooms = await _context.Rooms.Include(r=>r.Reservations).Where(r=>r.RoomTypeId==data.TypeId).ToListAsync();
        var availableRoom = typeRooms.FirstOrDefault(room =>
            !room.Reservations.Any(res => res.CheckInDate < userCheckOut && res.CheckOutDate > userCheckIn));
        
        Room? reservedRoom = null;
        var user = await _context.Users.FirstOrDefaultAsync(u=>u.Id==id);
        if (availableRoom == null)
        {
            return new ResultReservationDto
            {
                Result = false,
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
        return new ResultReservationDto
        {
            Result = true,
            Message = "The reservation was created",
            Item = userReservation,
            ReservedRoom = reservedRoom
            
        };
    }

    public async Task<ResultDto> EditReservation(UpdateReservationDto data, Guid id)
    {
        var userReservation = await _context.Reservations.Include(reserv=>reserv.Room).ThenInclude(r=>r.Type).FirstOrDefaultAsync(r=>r.Id==id);
        if (userReservation == null)
        {
            return new ResultDto
            {
                Result = false,
                Message = "The reservation is not found"
            };
        }
        var reservedRoom = await _context.Rooms.Include(r => r.Reservations).FirstOrDefaultAsync(r=>r.Id==userReservation.RoomId);
        var roomReservations = reservedRoom?.Reservations!;
        var newCheckIn = data.CheckIn == DateOnly.MinValue ? userReservation.CheckInDate:data.CheckIn;
        var newCheckOut = data.CheckOut == DateOnly.MinValue ? userReservation.CheckInDate:data.CheckOut;

        foreach (var reservation in roomReservations)
        {
            
            var checkOut = reservation.CheckOutDate;
            if (checkOut > newCheckIn && reservation.Id != id || newCheckOut < checkOut && reservation.Id != id)
            {
                return new ResultDto
                {
                    Message = "The range which you selected is occupied",
                    Result = false
                };
            }
        }

        userReservation.CheckInDate = newCheckIn;
        userReservation.CheckOutDate = newCheckOut;
        userReservation.TotalPrice = (newCheckOut.DayNumber - newCheckIn.DayNumber) * reservedRoom.Type.PricePerNight;

        await _context.SaveChangesAsync();
        return new ResultDto()
        {
            Result = true,
            Message = "The reservation was updated"
        };
    }

    public async Task<ResultDto> DeleteReservation(Guid id, string userId)
    {
        if (id == Guid.Empty)
        {
            return new ResultDto
            {
                Message = "There is no id of reservation",
                Result = false
            };
        }

        var reservation = await _context.Reservations.Include(u=>u.User).FirstOrDefaultAsync(r => r.Id == id);
        if (reservation == null)
        {
            return new ResultDto
            {
                Message = "The reservation is not found",
                Result = false
            };
        }

        var checkingUser = await _userManager.FindByIdAsync(userId);
        if (checkingUser == null)
        {
            return new ResultDto
            {
                Result = false,
                Message = "The user is not found"
            };
        }
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
            return new ResultDto
            {
                Message = "Access is denied",
                Result = false
            };
        }

        reservation.Status = Statuses.Canceled;
        await _context.SaveChangesAsync();
        return new ResultDto
        {
            Result = true,
            Message = "The reservation was canceled"
        };
    }
}