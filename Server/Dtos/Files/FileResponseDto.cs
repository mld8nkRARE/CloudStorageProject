namespace Server.Dtos.Files
{
    public record FileResponseDto(
    Guid Id,
    string FileName,
    string ContentType,
    long Size,
    DateTime UploadedAt,
    DateTime LastUpdateAt,
    Guid? FolderId);
}
