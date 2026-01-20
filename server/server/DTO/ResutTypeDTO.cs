namespace server.DTO;

public class ResutTypeDto<T>:ResultDto
{
    public List<T> Items { get; set; } = new List<T>();
}