using server.models;

namespace server.DTO;

public class PaginatedItemsDTO<T>
{
    public List<T> Items { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPage { get; set; }
}