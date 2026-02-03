using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;

namespace server.models;

public class User:IdentityUser
{
    public string Name { get; set; }
    public bool OnDuty { get; set; } = false;
    public string? Position { get; set; }
    public List<Reservation> Reservations { get; set; } = new();
    public AvatarUser? AvatarUser { get; set; }
    public string Role { get; set; } = Roles.User;
}