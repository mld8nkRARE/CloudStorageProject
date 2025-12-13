using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Dtos.Folders;
using Server.Extensions;
using Server.Services;

namespace Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/folders")]
    public class FolderController : ControllerBase
    {
        private readonly IFolderService _folderService;

        public FolderController(IFolderService folderService)
        {
            _folderService = folderService;
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> Create([FromBody] CreateFolderRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.GetUserId();
            var folderId = await _folderService.CreateAsync(dto, userId);
            return Ok(folderId);
        }

        [HttpGet("{folderId?}")]
        public async Task<ActionResult<FolderContentDto>> GetContent(Guid? folderId)
        {
            var userId = User.GetUserId();
            var content = await _folderService.GetContentAsync(folderId, userId);
            return Ok(content);
        }

        [HttpDelete("{folderId}")]
        public async Task<IActionResult> Delete(Guid folderId)
        {
            var userId = User.GetUserId();
            var deleted = await _folderService.DeleteAsync(folderId, userId);
            return deleted ? Ok() : NotFound();
        }

        [HttpPatch("{folderId}/move")]
        public async Task<IActionResult> MoveFolder(Guid folderId, [FromBody] MoveTargetDto moveDto)
        {
            var userId = User.GetUserId();
            var success = await _folderService.MoveFolderAsync(folderId, moveDto.TargetFolderId, userId);
            return success ? Ok() : BadRequest("Нельзя переместить папку");
        }

        [HttpPatch("file/{fileId}/move")]
        public async Task<IActionResult> MoveFile(Guid fileId, [FromBody] MoveTargetDto moveDto)
        {
            var userId = User.GetUserId();
            var success = await _folderService.MoveFileAsync(fileId, moveDto.TargetFolderId, userId);
            return success ? Ok() : BadRequest();
        }

        [HttpGet("{folderId}/path")]
        public async Task<ActionResult<List<FolderPathDto>>> GetFolderPath(Guid folderId)
        {
            var userId = User.GetUserId();
            var path = await _folderService.GetFolderPathAsync(folderId, userId);
            return Ok(path);
        }

        [HttpPatch("{folderId}/rename")]
        public async Task<IActionResult> RenameFolder(Guid folderId, [FromBody] RenameFolderDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.GetUserId();
            var success = await _folderService.RenameAsync(folderId, dto.NewName, userId);
            return success ? Ok() : BadRequest("Не удалось переименовать папку");
        }
    }
}
