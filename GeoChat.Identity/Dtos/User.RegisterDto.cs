using System.ComponentModel.DataAnnotations;

namespace GeoChat.Identity.Api.Dtos;

public record UserRegisterDto(
    [Required]
    [EmailAddress]
    string Email,

    [Required]
    string UserName,

    [Required]
    string Password,

    [Required]
    string PasswordConfirmation
);
