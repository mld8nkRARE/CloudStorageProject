using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Dtos.User;
using Server.Services.Interfaces;

namespace Server.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<UserService> _logger;

        public UserService(AppDbContext context, ILogger<UserService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<UserProfileDto> GetProfileAsync(Guid userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return null;

            var usedStorage = await _context.Files
                .Where(f => f.UserId == userId)
                .SumAsync(f => (long?)f.Size) ?? 0;

            var filesCount = await _context.Files
                .CountAsync(f => f.UserId == userId);

            return new UserProfileDto
            {
                Id = user.Id,
                Nickname = user.Nickname,
                Email = user.Email,
                CreatedAt = user.CreatedAt,
                UsedStorage = usedStorage,
                FilesCount = filesCount
            };
        }

        public async Task<StorageInfoDto> GetStorageInfoAsync(Guid userId)
        {
            var usedStorage = await _context.Files
                .Where(f => f.UserId == userId)
                .SumAsync(f => (long?)f.Size) ?? 0;

            return new StorageInfoDto
            {
                UsedBytes = usedStorage,
                MaxBytes = 100 * 1024 * 1024
            };
        }

        public async Task<bool> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return false;

            // Проверяем текущий пароль с помощью BCrypt
            if (!BCrypt.Net.BCrypt.Verify(currentPassword, user.PasswordHash))
                return false;

            // Хешируем новый пароль с помощью BCrypt
            var newPasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            user.SetPasswordHash(newPasswordHash);

            await _context.SaveChangesAsync();
            return true;
        }
    }
}