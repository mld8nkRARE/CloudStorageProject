namespace Server.Dtos.Folders
{
    public record FolderContentDto(
        Guid FolderId,
        string FolderName,
        Guid? ParentFolderId,
        IReadOnlyList<FolderItemDto> Items
    );
}
