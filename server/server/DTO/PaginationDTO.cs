namespace server.DTO;

public class PaginationDTO
{

    public int currentPage { get; set; } = 1;
    public Guid TypeId { get; set; } = Guid.Empty;

}