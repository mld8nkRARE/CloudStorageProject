using Client.Models.Folders;
using System;
using System.Threading.Tasks;

namespace Client.Services.Interfaces
{
    public interface IFolderService
    {
        Task<Guid> CreateAsync(CreateFolderRequest dto, Guid userId);
        Task<FolderContent> GetContentAsync(Guid? folderId, Guid userId);
        //Task<FolderTree> GetTreeAsync(Guid userId, Guid? rootFolderId = null);
        Task<bool> DeleteAsync(Guid folderId, Guid userId);
        Task<bool> MoveFolderAsync(Guid folderId, Guid? targetFolderId, Guid userId);
        Task<bool> MoveFileAsync(Guid fileId, Guid? targetFolderId, Guid userId);
    }

}

