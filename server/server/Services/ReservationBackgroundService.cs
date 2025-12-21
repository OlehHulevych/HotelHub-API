using Microsoft.EntityFrameworkCore;
using server.Data;
using server.models;

namespace server.Services;

public class ReservationBackgroundService:BackgroundService
{
    
    private readonly IServiceProvider _services;


    public ReservationBackgroundService(IServiceProvider services)
    {
        
        _services = services;

    }

    public async Task CheckIfExpired(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            using (var scope = _services.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var reservations = await _context.Reservations.ToListAsync();
                foreach (var reservation in reservations)
                {
                    if (DateOnly.FromDateTime(DateTime.UtcNow) > reservation.CheckOutDate)
                    {
                        reservation.Status = Statuses.Canceled;
                    }
                }

                await _context.SaveChangesAsync();
            }
            await Task.Delay(TimeSpan.FromHours(1), cancellationToken);
        }
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await this.CheckIfExpired(stoppingToken);
        
    }
}