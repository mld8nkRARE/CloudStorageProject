using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Dtos.Folders;
using Server.Models;

namespace Server.Services
{
    public class FolderService : IFolderService
    {
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;
        private readonly IFileService _fileService;

        public FolderService(AppDbContext db, IMapper mapper, IFileService fileService)
        {
            _db = db;
            _mapper = mapper;
            _fileService = fileService;
        }

        public async Task<Guid> CreateAsync(CreateFolderRequestDto dto, Guid userId)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("Имя папки не может быть пустым");

            if (dto.ParentFolderId.HasValue)
            {
                var parent = await _db.Folders
                    .FirstOrDefaultAsync(f => f.Id == dto.ParentFolderId.Value && f.UserId == userId);
                if (parent == null)
                    throw new UnauthorizedAccessException("Родительская папка не найдена или недоступна");
            }

            var folder = new Folder
            {
                Name = dto.Name.Trim(),
                UserId = userId,
                ParentFolderId = dto.ParentFolderId
            };

            _db.Folders.Add(folder);
            await _db.SaveChangesAsync();

            return folder.Id;
        }

        public async Task<FolderContentDto> GetContentAsync(Guid? folderId, Guid userId)
        {
            // Проверка доступа к папке
            if (folderId.HasValue)
            {
                var folder = await _db.Folders
                    .FirstOrDefaultAsync(f => f.Id == folderId.Value && f.UserId == userId);
                if (folder == null)
                    throw new UnauthorizedAccessException("Папка не найдена или доступ запрещён");
            }

            // Получаем подпапки
            var subFolders = await _db.Folders
                .Where(f => f.UserId == userId && f.ParentFolderId == folderId)
                .OrderBy(f => f.Name)
                .ProjectTo<FolderItemDto>(_mapper.ConfigurationProvider)
                //.Select(f => new FolderItemDto(f.Id, f.Name, "folder", f.CreatedAt, null))
                .ToListAsync();

            // Получаем файлы
            var files = await _fileService.GetListAsync(userId, folderId);
            var fileItems = _mapper.Map<List<FolderItemDto>>(files);
            /*var fileItems = files.Select(f => new FolderItemDto(
                f.Id, f.FileName, "file", f.UploadedAt, f.Size)).ToList();*/

            // Объединяем и сортируем по имени
            var items = subFolders
                .Concat(fileItems)
                .OrderBy(i => i.Name)
                .ToList();

            var currentFolderName = folderId.HasValue
                ? (await _db.Folders.FirstAsync(f => f.Id == folderId.Value)).Name
                : "Корневая папка";

            return new FolderContentDto(folderId ?? Guid.Empty, currentFolderName, folderId, items);
        }

        public async Task<bool> DeleteAsync(Guid folderId, Guid userId)
        {
            var folder = await _db.Folders
                .Include(f => f.SubFolders)
                .Include(f => f.Files)
                .FirstOrDefaultAsync(f => f.Id == folderId && f.UserId == userId);

            if (folder == null) return false;

            // Рекурсивно удаляем всё внутри
            await DeleteFolderRecursively(folder);

            await _db.SaveChangesAsync();
            return true;
        }

        private async Task DeleteFolderRecursively(Folder folder)
        {
            // Удаляем вложенные папки
            foreach (var subFolder in folder.SubFolders.ToList())
            {
                await DeleteFolderRecursively(subFolder);
            }

            // Удаляем файлы
            foreach (var file in folder.Files.ToList())
            {
                await _fileService.DeleteAsync(file.Id, folder.UserId);
            }

            _db.Folders.Remove(folder);
        }

        public async Task<bool> MoveFolderAsync(Guid folderId, Guid? targetFolderId, Guid userId)
        {
            var folder = await _db.Folders.FirstOrDefaultAsync(f => f.Id == folderId && f.UserId == userId);
            if (folder == null) return false;

            // Нельзя переместить в самого себя или в потомка
            if (targetFolderId == folderId || await IsDescendant(folderId, targetFolderId))
                return false;

            if (targetFolderId.HasValue)
            {
                var target = await _db.Folders
                    .FirstOrDefaultAsync(f => f.Id == targetFolderId.Value && f.UserId == userId);
                if (target == null) return false;
            }

            folder.ParentFolderId = targetFolderId;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> MoveFileAsync(Guid fileId, Guid? targetFolderId, Guid userId)
        {
            if (targetFolderId.HasValue)
            {
                var target = await _db.Folders
                    .FirstOrDefaultAsync(f => f.Id == targetFolderId.Value && f.UserId == userId);
                if (target == null) return false;
            }

            return await _fileService.MoveToFolderAsync(fileId, targetFolderId, userId);
        }

        private async Task<bool> IsDescendant(Guid ancestorId, Guid? descendantId)
        {
            if (!descendantId.HasValue) return false;

            var current = await _db.Folders.FindAsync(descendantId.Value);
            while (current != null)
            {
                if (current.Id == ancestorId) return true;
                current = current.ParentFolderId.HasValue
                    ? await _db.Folders.FindAsync(current.ParentFolderId.Value)
                    : null;
            }
            return false;
        }

        public async Task<List<FolderPathDto>> GetFolderPathAsync(Guid folderId, Guid userId)
        {
            var path = new List<FolderPathDto>();
            var currentFolder = await _db.Folders
                .Where(f => f.Id == folderId && f.UserId == userId)
                .FirstOrDefaultAsync();

            while (currentFolder != null)
            {
                path.Insert(0, new FolderPathDto
                {
                    Id = currentFolder.Id,
                    Name = currentFolder.Name
                });

                currentFolder = currentFolder.ParentFolderId.HasValue
                    ? await _db.Folders
                        .Where(f => f.Id == currentFolder.ParentFolderId.Value && f.UserId == userId)
                        .FirstOrDefaultAsync()
                    : null;
            }

            return path;
        }

        public async Task<bool> RenameAsync(Guid folderId, string newName, Guid userId)
        {
            if (string.IsNullOrWhiteSpace(newName))
                return false;

            var folder = await _db.Folders
                .Where(f => f.Id == folderId && f.UserId == userId)
                .FirstOrDefaultAsync();

            if (folder == null)
                return false;

            folder.Name = newName.Trim();
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
