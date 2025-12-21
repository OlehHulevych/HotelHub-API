namespace server.models;

public class Details
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public List<string> Norishment { get; set; }
    public List<string> Spa { get; set; }
    public List<string> view { get; set; }
    public int Capacity { get; set; }
    public Guid RoomTypeId { get; set; }
    public RoomType RoomType { get; set; }
}