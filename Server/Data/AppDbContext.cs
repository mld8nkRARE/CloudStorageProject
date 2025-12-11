using Microsoft.EntityFrameworkCore;
using Server.Models;
namespace Server.Data
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<FileModel> Files => Set<FileModel>();
        public DbSet<Folder> Folders => Set<Folder>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Уникальные индексы (без NOCASE — просто и работает)
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Nickname)
                .IsUnique();

            // StoredFileInfo: первичный ключ по Guid
            modelBuilder.Entity<FileModel>()
            .HasIndex(f => f.Id)
            .IsUnique();

            // Связь один-ко-многим: User → Files (каскадное удаление)
            modelBuilder.Entity<FileModel>()
                .HasOne(f => f.User)
                .WithMany(u => u.Files)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Индекс по UserId для быстрых запросов списка файлов
            modelBuilder.Entity<FileModel>()
                .HasIndex(f => f.UserId);
            modelBuilder.Entity<Folder>()
            .HasIndex(f => f.Id)
            .IsUnique();

            // Связь Folder ↔ SubFolders (рекурсивная)
            modelBuilder.Entity<Folder>()
                .HasMany(f => f.SubFolders)
                .WithOne(f => f.ParentFolder)
                .HasForeignKey(f => f.ParentFolderId)
                .OnDelete(DeleteBehavior.Cascade);

            // Связь Folder ↔ Files
            modelBuilder.Entity<FileModel>()
                .HasOne(f => f.Folder)
                .WithMany(f => f.Files)
                .HasForeignKey(f => f.FolderId)
                .OnDelete(DeleteBehavior.SetNull);

            // Обновим внешние ключи для User
            modelBuilder.Entity<FileModel>()
                .HasOne(f => f.User)
                .WithMany(u => u.Files)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Folder>()
                .HasOne(f => f.User)
                .WithMany(u => u.Folders)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
