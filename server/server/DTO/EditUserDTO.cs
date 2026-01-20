using System.ComponentModel.DataAnnotations;

namespace server.DTO;

public  class EditUserDtO
{
    [EmailAddress] public string? Email { get; set; } = null;
    public string? Name { get; set; } = null;
    public IFormFile? PhotoFile { get; set; } = null;
}