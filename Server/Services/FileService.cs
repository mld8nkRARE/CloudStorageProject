using Server.Data;
using AutoMapper;
using Server.Dtos.Files;
using Server.Models;
using Microsoft.EntityFrameworkCore;


namespace Server.Services
{
    public class FileService : IFileService
    {
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;
        private readonly string _storagePath;

        public FileService(AppDbContext db, IWebHostEnvironment env, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
            _storagePath = Path.Combine(env.ContentRootPath, "uploads");
            Directory.CreateDirectory(_storagePath);
        }

        public async Task<FileResponseDto> UploadAsync(IFormFile file, Guid userId, Guid? folderId = null)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("Файл не выбран");
            if (folderId.HasValue)
            {
                var folder = await _db.Folders
                    .FirstOrDefaultAsync(f => f.Id == folderId.Value && f.UserId == userId);

                if (folder == null)
                    throw new UnauthorizedAccessException("Папка не найдена или доступ запрещён");
            }
            var fileModel = new FileModel
            {
                FileName = file.FileName,
                ContentType = file.ContentType,
                Size = file.Length,
                UserId = userId,
                FolderId = folderId,
                UploadedAt = DateTime.UtcNow,
                LastUpdateAt = DateTime.UtcNow
            };

            // Id генерируется автоматически EF Core (Guid.NewGuid())

            var physicalPath = Path.Combine(_storagePath, fileModel.GetPhysicalFileName());

            await using (var stream = new FileStream(physicalPath, FileMode.Create))
                await file.CopyToAsync(stream);

            _db.Files.Add(fileModel);
            await _db.SaveChangesAsync();

            return _mapper.Map<FileResponseDto>(fileModel);
        }

        public async Task<IReadOnlyList<FileResponseDto>> GetListAsync(Guid userId, Guid? folderId = null)
        {
            var query = _db.Files
                .Where(f => f.UserId == userId);

            if (folderId.HasValue)
                query = query.Where(f => f.FolderId == folderId);
            else
                query = query.Where(f => f.FolderId == null); // файлы в корне

            var files = await query
                .OrderByDescending(f => f.UploadedAt)
                .ToListAsync();

            return _mapper.Map<List<FileResponseDto>>(files);
        }

        public async Task<FileDownloadDto?> GetForDownloadAsync(Guid fileId, Guid userId)
        {
            var file = await _db.Files
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.Id == fileId && f.UserId == userId);

            if (file == null)
                return null;

            var physicalPath = Path.Combine(_storagePath, file.GetPhysicalFileName());

            if (!System.IO.File.Exists(physicalPath))
                return null;
            var stream = System.IO.File.OpenRead(physicalPath);

            return new FileDownloadDto(stream, file.FileName, file.ContentType);
        }

        public async Task<bool> DeleteAsync(Guid fileId, Guid userId)
        {
            var file = await _db.Files
                .FirstOrDefaultAsync(f => f.Id == fileId && f.UserId == userId);


            if (file == null) return false;

            var physicalPath = Path.Combine(_storagePath, file.GetPhysicalFileName());

            if (System.IO.File.Exists(physicalPath))
                System.IO.File.Delete(physicalPath);

            _db.Files.Remove(file);
            await _db.SaveChangesAsync();

            return true;
        }
        public async Task<long> GetUserUsedBytesAsync(Guid userId)
        {
            return await _db.Files
                .Where(f => f.UserId == userId)
                .SumAsync(f => f.Size);
        }
        public async Task<bool> MoveToFolderAsync(Guid fileId, Guid? targetFolderId, Guid userId)
        {
            var file = await _db.Files
                .FirstOrDefaultAsync(f => f.Id == fileId && f.UserId == userId);

            if (file == null) return false;

            if (targetFolderId.HasValue)
            {
                var folder = await _db.Folders
                    .FirstOrDefaultAsync(f => f.Id == targetFolderId.Value && f.UserId == userId);
                if (folder == null) return false;
            }

            file.FolderId = targetFolderId;
            file.LastUpdateAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return true;
        }
    }
}
