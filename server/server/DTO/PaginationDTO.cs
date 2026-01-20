namespace server.DTO;

public class PaginationDto
{

    public int CurrentPage { get; set; } = 1;
    public Guid TypeId { get; set; } = Guid.Empty;

}