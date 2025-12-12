using Client.Models.Folders;
using System;
using System.Threading.Tasks;

namespace Client.Services.Interfaces
{
    public interface IFolderService
    {
        Task<Guid> CreateAsync(CreateFolderRequest dto);
        Task<FolderContent> GetContentAsync(Guid? folderId);
        //Task<FolderTree> GetTreeAsync(Guid userId, Guid? rootFolderId = null);
        Task<bool> DeleteAsync(Guid folderId);
        Task<bool> MoveFolderAsync(Guid folderId, Guid? targetFolderId);
        Task<bool> MoveFileAsync(Guid fileId, Guid? targetFolderId);
    }

}

