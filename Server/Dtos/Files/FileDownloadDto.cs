namespace Server.Dtos.Files
{
    public record FileDownloadDto
    (
         Stream FileStream,
         string FileName,
         string ContentType
    );
}
