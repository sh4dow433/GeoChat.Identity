
using Microsoft.AspNetCore.Identity;

namespace GeoChat.Identity.Api.Entities;

public class AppUser : IdentityUser
{
    public DateTime LastUpdated { get; set; }
}
