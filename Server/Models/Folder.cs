using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Models
{
    public class Folder
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required, StringLength(255)]
        public string Name { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Внешний ключ: кто владелец папки
        [Required]
        public Guid UserId { get; set; }

        // Навигационное свойство: владелец
        [ForeignKey(nameof(UserId))]
        public User User { get; set; } = null!;

        // Внешний ключ: в какой папке находится эта папка (null = корень)
        public Guid? ParentFolderId { get; set; }

        // Навигационное свойство: родительская папка
        [ForeignKey(nameof(ParentFolderId))]
        public Folder? ParentFolder { get; set; }

        // Навигационные свойства: вложенные папки и файлы
        public List<Folder> SubFolders { get; set; } = new();
        public List<FileModel> Files { get; set; } = new();
    }
}
