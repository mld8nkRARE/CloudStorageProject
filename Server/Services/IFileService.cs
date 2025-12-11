
using Server.Dtos.Files;
namespace Server.Services
{
    public interface IFileService
    {
        /// <summary>
        /// Загружает файл и возвращает его модель
        /// </summary>
        Task<FileResponseDto> UploadAsync(IFormFile file, Guid userId, Guid? folderId = null);

        /// <summary>
        /// Возвращает список файлов пользователя
        /// </summary>
        Task<IReadOnlyList<FileResponseDto>> GetListAsync(Guid userId, Guid? folderId = null);

        /// <summary>
        /// Возвращает информацию о файле для скачивания (проверяет права)
        /// </summary>
        Task<FileDownloadDto?> GetForDownloadAsync(Guid fileId, Guid userId);

        /// <summary>
        /// Удаляет файл (проверяет права)
        /// </summary>  
        Task<bool> DeleteAsync(Guid fileId, Guid userId);

        /// <summary>
        /// Возвращает общий размер файлов пользователя
        /// </summary>
        Task<long> GetUserUsedBytesAsync(Guid userId);

        /// <summary>
        /// Перемещение файлов в папку
        /// </summary>
        Task<bool> MoveToFolderAsync(Guid fileId, Guid? targetFolderId, Guid userId);

    }
}
