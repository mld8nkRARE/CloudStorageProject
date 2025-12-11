using System.ComponentModel.DataAnnotations;

namespace Server.Dtos.Folders
{
    public record CreateFolderRequestDto
    (
        [Required]
        [StringLength(255, MinimumLength = 1)]
        string Name,
        Guid? ParentFolderId = null
    );
}
