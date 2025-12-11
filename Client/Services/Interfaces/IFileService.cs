using Client.Models.File;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client.Services.Interfaces
{
    public interface IFileService
    {
        Task<List<FileDto>> GetFilesAsync();
        Task<bool> UploadFileAsync(string filePath);
        Task<bool> DownloadFileAsync(Guid id, string savePath);
        Task<bool> DeleteFileAsync(Guid id);
    }
    //public interface IFileService
    //{
    //    Task<IEnumerable<FileDto>> GetFilesAsync(CancellationToken ct = default);
    //    Task<bool> UploadFileAsync(string filePath, IProgress<long>? progress = null, CancellationToken ct = default);
    //    Task<bool> DownloadFileAsync(Guid fileId, string saveToPath, IProgress<long>? progress = null, CancellationToken ct = default);
    //    Task<bool> DeleteFileAsync(Guid fileId, CancellationToken ct = default);
    //}
}
