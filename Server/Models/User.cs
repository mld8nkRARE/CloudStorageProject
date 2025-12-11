using System.ComponentModel.DataAnnotations;

namespace Server.Models
{
    public class User
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required,StringLength(50)]
        public string Nickname { get; set; } = string.Empty;

        [Required,EmailAddress,StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; private set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Навигационное свойство — всё ок
        public List<FileModel> Files { get; set; } = new();
        public List<Folder> Folders { get; set; } = new();

        //только сервер может менять хеш
        public void SetPasswordHash(string hash)
        {
            PasswordHash = hash;
        }
    }
}
