using server.models;

namespace server.DTO;

public class PaginatedItemsDto<T>
{
    public List<T> Items { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPage { get; set; }
    public int? TotalLength { get; set; }
    public int? CanceledLength { get; set; }
    public int? ActiveLength { get; set; }
}