using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Server.Data;
using Server.Dtos.Auth;
using Server.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
namespace Server.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;

        public AuthService(AppDbContext db, IConfiguration config, IMapper mapper)
        {
            _db = db;
            _config = config;
            _mapper = mapper;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto dto)
        {
            // Проверки
            if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
                throw new InvalidOperationException("Email или пароль пустые");

            if (await _db.Users.AnyAsync(u => u.Nickname == dto.Nickname))
                throw new InvalidOperationException("Этот никнейм уже занят");

            if (await _db.Users.AnyAsync(u => u.Email == dto.Email))
                throw new InvalidOperationException("Пользователь с таким email уже существует");

            var user = new User
            {
                Email = dto.Email.Trim().ToLower(),
                Nickname = dto.Nickname.Trim()
            };
            user.SetPasswordHash(BCrypt.Net.BCrypt.HashPassword(dto.Password));

            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            var token = GenerateJwtToken(user);
            return new AuthResponseDto(token.Token, token.ExpiresAt, user.Id, user.Nickname);
            
        }

        public async Task<AuthResponseDto?> LoginAsync(LoginRequestDto dto)
        {
            var user = await _db.Users
                .FirstOrDefaultAsync(u => u.Email == dto.Email.Trim().ToLower());

            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Неверный email или пароль");

            var token = GenerateJwtToken(user);
            return new AuthResponseDto(token.Token, token.ExpiresAt, user.Id, user.Nickname);
        }

        private (string Token, DateTime ExpiresAt) GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim(ClaimTypes.Name, user.Nickname ?? string.Empty)
            };

            var keyString = _config["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key не создан");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expires = DateTime.UtcNow.AddHours(
                int.TryParse(_config["Jwt:ExpiresHours"], out var h) ? h : 24);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds);

            return (new JwtSecurityTokenHandler().WriteToken(token),expires);
        }
    }
}
