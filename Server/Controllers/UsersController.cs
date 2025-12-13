using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Server.Dtos.User;
using Server.Extensions;
using Server.Services.Interfaces;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserService userService, ILogger<UsersController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpGet("profile")]
        public async Task<ActionResult<UserProfileDto>> GetProfile()
        {
            try
            {
                var userId = User.GetUserId();
                var profile = await _userService.GetProfileAsync(userId);
                return profile != null ? Ok(profile) : NotFound();
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка получения профиля");
                return StatusCode(500);
            }
        }

        [HttpGet("storage")]
        public async Task<ActionResult<StorageInfoDto>> GetStorageInfo()
        {
            try
            {
                var userId = User.GetUserId();
                var storageInfo = await _userService.GetStorageInfoAsync(userId);
                return Ok(storageInfo);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка получения информации о хранилище");
                return StatusCode(500);
            }
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var userId = User.GetUserId();
                var result = await _userService.ChangePasswordAsync(
                    userId, dto.CurrentPassword, dto.NewPassword);

                return result ? Ok() : BadRequest("Неверный пароль");
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка смены пароля");
                return StatusCode(500);
            }
        }
    }
}
