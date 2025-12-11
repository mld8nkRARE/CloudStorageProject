using System.ComponentModel.DataAnnotations;

namespace Server.Dtos.Auth
{
    public record LoginRequestDto(
        [Required]
        [EmailAddress]
        string Email,
        [Required]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Длина пароля должна быть от 8 до 100 символов")]
        string Password
    );
}
