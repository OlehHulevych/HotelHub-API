using System.Xml;
using Microsoft.EntityFrameworkCore;
using server.Data;
using server.models;

namespace server.Services;

public class RoomBackgroundService:BackgroundService
{
    private readonly IServiceProvider _services;

    public RoomBackgroundService(IServiceProvider services)
    {
        _services = services;
    }

    public async Task CheckIfOccupied(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            using (var scope = _services.CreateScope())
            {
                
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var rooms = await context.Rooms.Include(r => r.Reservations).Where(r=>r.Reservations.Any()).ToListAsync();
                foreach (Room room in rooms)
                {
                    foreach (Reservation reservation in room.Reservations)
                    {
                        DateOnly currentDate = DateOnly.FromDateTime(DateTime.Today);
                        if (currentDate >= reservation.CheckInDate)
                        {
                            room.Status = RoomStatus.Occupied;
                        }

                        if (currentDate > reservation.CheckOutDate)
                        {
                            room.Status = RoomStatus.Free;
                        }
                    }
                    
                }

                await context.SaveChangesAsync();

            }

            await Task.Delay(TimeSpan.FromHours(1), cancellationToken);
        }
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await this.CheckIfOccupied(stoppingToken);
    }
}