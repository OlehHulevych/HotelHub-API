using server.models;

namespace server.Entities;

public struct Report
{
    public int OccupiedRooms { get; set; }
    public int AvailableRooms { get; set; }
    public List<User> Guests { get; set; }
    public List<User> Workers { get; set; }
    public int Reservations { get; set; }
    public int TotalRevenue { get; set; }

    public Report(int occupiedRooms, int availableRooms, List<User> guests, List<User> workers, int reservations, int totalRevenue)
    {
        OccupiedRooms = occupiedRooms;
        AvailableRooms = availableRooms;
        Guests = guests;
        Workers = workers;
        Reservations = reservations;
        TotalRevenue = totalRevenue;

    }
}