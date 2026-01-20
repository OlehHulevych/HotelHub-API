using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace server.DTO;

public class RoomDto
{
    
    [Required]
    public Guid RoomTypeId { get; set; }  
    public int Number { get; set; } = 0;
}