namespace server.DTO;

public class ResutTypeDto<T>:ResultDTO
{
    public List<T> Items { get; set; } = new List<T>();
}