using Server.Dtos.Folders;
namespace Server.Services
{
    public interface IFolderService
    {
        /// <summary>
        /// Создаёт новую папку
        /// </summary>
        Task<Guid> CreateAsync(CreateFolderRequestDto dto, Guid userId);

        /// <summary>
        /// Получает список папок и файлов в указанной папке (или в корне, если folderId = null)
        /// </summary>
        Task<FolderContentDto> GetContentAsync(Guid? folderId, Guid userId);

        /// <summary>
        /// Удаляет папку и всё её содержимое (вложенные папки и файлы)
        /// </summary>
        Task<bool> DeleteAsync(Guid folderId, Guid userId);

        /// <summary>
        /// Перемещает файл в другую папку
        /// </summary>
        Task<bool> MoveFileAsync(Guid fileId, Guid? targetFolderId, Guid userId);

        /// <summary>
        /// Перемещает папку в другую папку
        /// </summary>
        Task<bool> MoveFolderAsync(Guid folderId, Guid? targetFolderId, Guid userId);

        Task<List<FolderPathDto>> GetFolderPathAsync(Guid folderId, Guid userId);
        Task<bool> RenameAsync(Guid folderId, string newName, Guid userId);
    }
}
