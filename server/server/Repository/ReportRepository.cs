using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using server.Data;
using server.DTO;
using server.Entities;
using server.IRepositories;
using server.models;
using server.ResponseDTO;

namespace server.Repository;

public class ReportRepository : IReportRepository
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> _userManager;

    public ReportRepository(ApplicationDbContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    public async Task<ResultDto> GetReport()
    {
        var occupiedRooms = await _context.Rooms.Include(r=>r.Reservations).Where(r=>r.Reservations.Any()).CountAsync();
        var availableRooms = await _context.Rooms.Include(r=>r.Reservations).Where(r=>!r.Reservations.Any()).CountAsync();
        var reservations = await _context.Reservations.ToListAsync();
        var reservationsCount = await _context.Reservations.CountAsync();
        var users = await _userManager.Users.Include(u=>u.AvatarUser).ToListAsync();
        int totalRevenue = 0;
        List<UserDTO> workers = await (from u in _context.Users.AsNoTracking()
                join a in _context.AvatarUsers on u.Id equals a.UserId
                join ur in _context.UserRoles on u.Id equals ur.UserId
                join r in _context.Roles on ur.RoleId equals r.Id
                where r.Name == "STAFF"
                select new UserDTO (u.Id, u.Name, u.Email, u.Position, u.OnDuty, a.AvatarPath, u.Banned)
                ).ToListAsync();
        List<UserDTO> guests = await (from u in _context.Users.AsNoTracking()
                join a in _context.AvatarUsers on u.Id equals a.UserId
                join ur in _context.UserRoles on u.Id equals ur.UserId
                join r in _context.Roles on ur.RoleId equals r.Id
                where r.Name == "USER" && _context.UserRoles.Count(x=>x.UserId==u.Id)==1
                select new UserDTO (u.Id, u.Name, u.Email,u.Position, u.OnDuty, a.AvatarPath, u.Banned)
            ).ToListAsync();

        foreach (Reservation reservation in reservations )
        {
            totalRevenue += reservation.TotalPrice;
        }
        

        Report report = new Report(occupiedRooms, availableRooms,guests, workers, reservationsCount, totalRevenue);

        return new ResultDto
        {
            Result = true,
            Message = "Report is created",
            Item = report
        };
    }
}
