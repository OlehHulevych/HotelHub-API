namespace server.ResponseDTO;

public class ReservationAdminDTO
{
    public Guid Id { get; set; }
    public string GuestName { get; set; }
    public string Status { get; set; }
    public DateOnly CheckInDate { get; set; }
    public DateOnly CheckOutDate { get; set; }

    public ReservationAdminDTO(Guid id, string guestName, string status, DateOnly checkInDate, DateOnly checkOutDate)
    {
        Id = id;
        GuestName = guestName;
        Status = status;
        CheckInDate = checkInDate;
        CheckOutDate = checkOutDate;
    }
}