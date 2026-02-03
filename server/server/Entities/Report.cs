using server.models;
using server.ResponseDTO;

namespace server.Entities;

public struct Report
{
    public int OccupiedRooms { get; set; }
    public int AvailableRooms { get; set; }
    public List<UserDTO> Guests { get; set; }
    public List<UserDTO> Workers { get; set; }
    public int Reservations { get; set; }
    public int TotalRevenue { get; set; }

    public Report(int occupiedRooms, int availableRooms, List<UserDTO> guests, List<UserDTO> workers, int reservations, int totalRevenue)
    {
        OccupiedRooms = occupiedRooms;
        AvailableRooms = availableRooms;
        Guests = guests;
        Workers = workers;
        Reservations = reservations;
        TotalRevenue = totalRevenue;

    }
}