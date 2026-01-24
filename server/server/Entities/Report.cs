namespace server.Entities;

public struct Report
{
    public int OccupiedRooms { get; set; }
    public int AvailableRooms { get; set; }
    public int Guests { get; set; }
    public int Workers { get; set; }
    public int Reservations { get; set; }
    public int TotalRevenue { get; set; }

    public Report(int occupiedRooms, int availableRooms, int guests, int workers, int reservations, int totalRevenue)
    {
        OccupiedRooms = occupiedRooms;
        AvailableRooms = availableRooms;
        Guests = guests;
        Workers = workers;
        Reservations = reservations;
        TotalRevenue = totalRevenue;

    }
}