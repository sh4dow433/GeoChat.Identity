using GeoChat.Identity.Api.Entities;

namespace GeoChat.Identity.Api.Services
{
    public interface ITokenGenerator
    {
        Task<string> GenerateTokenAsync(AppUser user);
    }
}