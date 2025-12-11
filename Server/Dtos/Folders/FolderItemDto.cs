namespace Server.Dtos.Folders
{
    public class FolderItemDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // "folder" или "file"
        public long? Size { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
