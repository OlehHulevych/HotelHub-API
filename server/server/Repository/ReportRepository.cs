using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using server.Data;
using server.DTO;
using server.Entities;
using server.IRepositories;
using server.models;

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
        var users = await _userManager.Users.ToListAsync();
        int totalRevenue = 0;
        List<User> workers = new List<User>();
        List<User> guests = new List<User>();

        foreach (Reservation reservation in reservations )
        {
            totalRevenue += reservation.TotalPrice;
        }
        foreach (var user in users )
        {
            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Contains("ADMIN"))
            {
                workers.Add(user);
            }
            else
            {
                guests.Add(user);
            }
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
