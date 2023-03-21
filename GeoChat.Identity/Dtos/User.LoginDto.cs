using System.ComponentModel.DataAnnotations;

namespace GeoChat.Identity.Api.Dtos;

public record UserLoginDto(
    [Required]
    [EmailAddress]
    string Email,

    [Required]
    string Password
);
