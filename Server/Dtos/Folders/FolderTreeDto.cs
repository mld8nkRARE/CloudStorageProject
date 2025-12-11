using Server.Dtos.Files;

namespace Server.Dtos.Folders
{
    public record FolderTreeDto
    (
        Guid Id,
        string Name,
        IReadOnlyList<FolderTreeDto> SubFolders,
        IReadOnlyList<FileResponseDto> Files
    );
}
