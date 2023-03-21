using GeoChat.Identity.Api.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GeoChat.Identity.Api.Services;

public class TokenGenerator : ITokenGenerator
{
    private readonly IConfiguration _configuration;
    private readonly UserManager<AppUser> _userManager;

    public TokenGenerator(IConfiguration configuration, UserManager<AppUser> userManager)
    {
        _configuration = configuration;
        _userManager = userManager;
    }
    public async Task<string> GenerateTokenAsync(AppUser user)
    {
        var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // jti = token id
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),// issued at DateTime
                new Claim("UserName", user.UserName!),
                new Claim("Email", user.Email!),
                new Claim("Id", user.Id.ToString())
            };
        var usersClaims = await _userManager.GetClaimsAsync(user);
        if (usersClaims != null)
        {
            claims.AddRange(usersClaims);
        }
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var signInCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken
            (
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: signInCredentials
            );
        var tokenHandler = new JwtSecurityTokenHandler();
        return tokenHandler.WriteToken(token);
    }
}
