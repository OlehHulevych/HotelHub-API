namespace server.ResponseDTO;

public class ReservationDTO
{
    public Guid Id { get; set; }
    public string TypeName { get; set; }
    public IEnumerable<string> Photos { get; set; }
    public string Status { get; set; }
    public DateOnly CheckInDate { get; set; }
    public DateOnly CheckOutDate { get; set; }
    public int TotalPrice { get; set; }

    public ReservationDTO(Guid id,  string status, DateOnly checkInDate, DateOnly checkOutDate, IEnumerable<string> photos, string typeName, int totalPrice)
    {
        Id = id;
        Status = status;
        Photos = photos;
        TypeName = typeName;
        CheckInDate = checkInDate;
        CheckOutDate = checkOutDate;
        TotalPrice = totalPrice;

    }
}