using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Dtos.Files;
using Server.Models;
using Server.Services;
using System.Security.Claims;
using Server.Extensions;
namespace Server.Controllers
{
    [ApiController]
    [Route("api/files")]
    [Authorize]
    public class FilesController : ControllerBase
    {
        private readonly IFileService _fileService;

        public FilesController(IFileService fileService) => _fileService = fileService;

        [HttpPost("upload")]
        [RequestSizeLimit(1_000_000_000)]
        public async Task<ActionResult<FileResponseDto>> Upload(
            IFormFile file,
            [FromForm] Guid? folderId)
        {
            var userId = User.GetUserId(); // твой extension
            var result = await _fileService.UploadAsync(file, userId, folderId);
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<FileResponseDto>>> GetList([FromQuery] Guid? folderId)
        {
            var userId = User.GetUserId();
            var files = await _fileService.GetListAsync(userId, folderId);
            return Ok(files);
        }

        [HttpGet("{fileId}/download")]
        public async Task<IActionResult> Download(Guid fileId)
        {
            var userId = User.GetUserId();
            var file = await _fileService.GetForDownloadAsync(fileId, userId);

            if (file == null) return NotFound();

            return File(file.FileStream, file.ContentType, file.FileName);
        }

        [HttpDelete("{fileId}")]
        public async Task<IActionResult> Delete(Guid fileId)
        {
            var userId = User.GetUserId();
            var deleted = await _fileService.DeleteAsync(fileId, userId);
            return deleted ? Ok() : NotFound();
        }
    }
}
