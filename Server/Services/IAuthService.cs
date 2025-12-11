using Server.Dtos.Auth;
using Server.Models;
namespace Server.Services
{
    public interface IAuthService
    {
        /// <summary>
        /// Регистрирует пользователя. Возвращает (success, errorMessage, userId).
        /// </summary>
        Task<AuthResponseDto> RegisterAsync(RegisterRequestDto dto);

        /// <summary>
        /// Аутентификация — возвращает AuthResponseDto (token + expiry) или null, если не удалось.
        /// </summary>
        Task<AuthResponseDto?> LoginAsync(LoginRequestDto dto);
    }
}
