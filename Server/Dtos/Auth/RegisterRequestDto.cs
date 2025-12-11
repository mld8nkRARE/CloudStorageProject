using System.ComponentModel.DataAnnotations;

namespace Server.Dtos.Auth
{
    public record RegisterRequestDto(
        [Required]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Длина имени должна быть от 3 до 50 символов")]
        string Nickname,
        [Required]
        [EmailAddress]
        string Email,
        [Required]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Длина пароля должна быть от 8 до 100 символов")]
        string Password
    );
}

