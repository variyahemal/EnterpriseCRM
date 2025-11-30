using Asp.Versioning;
using EnterpriseCRM.API.DTOs.Media;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EnterpriseCRM.Infrastructure.Persistence;
using EnterpriseCRM.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace EnterpriseCRM.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize(Roles = "Admin,Editor")] // Base authorization for MediaController
    public class MediaController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IWebHostEnvironment _environment;
        private const string UploadsFolder = "uploads";
        private const long MaxFileSize = 50 * 1024 * 1024; // 50MB

        public MediaController(ApplicationDbContext context, UserManager<AppUser> userManager, IWebHostEnvironment environment)
        {
            _context = context;
            _userManager = userManager;
            _environment = environment;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Editor")]
        public async Task<ActionResult<IEnumerable<MediaFileDto>>> ListMedia(
            [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (page < 1 || pageSize < 1)
            {
                return BadRequest("Page and pageSize must be positive integers.");
            }

            var mediaFiles = await _context.MediaFiles
                .Where(m => !m.IsDeleted)
                .OrderByDescending(m => m.CreatedOn)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(m => new MediaFileDto
                {
                    Id = m.Id,
                    FileName = m.FileName,
                    MimeType = m.MimeType,
                    Url = m.Url,
                    Size = m.Size,
                    UploadedBy = m.UploadedBy,
                    CreatedAt = m.CreatedOn
                })
                .ToListAsync();

            return Ok(mediaFiles);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Editor")]
        public async Task<ActionResult<MediaFileDto>> GetMediaFile(string id)
        {
            var mediaFile = await _context.MediaFiles
                .Where(m => m.Id == id && !m.IsDeleted)
                .Select(m => new MediaFileDto
                {
                    Id = m.Id,
                    FileName = m.FileName,
                    MimeType = m.MimeType,
                    Url = m.Url,
                    Size = m.Size,
                    UploadedBy = m.UploadedBy,
                    CreatedAt = m.CreatedOn
                })
                .FirstOrDefaultAsync();

            if (mediaFile == null)
            {
                return NotFound("Media file not found.");
            }

            return Ok(mediaFile);
        }

        [HttpPost("upload")]
        [Authorize(Roles = "Admin,Editor")]
        public async Task<ActionResult<MediaFileDto>> UploadMedia([FromForm] UploadMediaDto uploadMediaDto)
        {
            if (uploadMediaDto.File == null || uploadMediaDto.File.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            if (uploadMediaDto.File.Length > MaxFileSize)
            {
                return BadRequest($"File size exceeds the maximum allowed size of {MaxFileSize / (1024 * 1024)}MB.");
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized("User is not authenticated.");
            }

            // Create uploads directory if it doesn't exist
            var uploadsPath = Path.Combine(_environment.WebRootPath ?? _environment.ContentRootPath, UploadsFolder);
            if (!Directory.Exists(uploadsPath))
            {
                Directory.CreateDirectory(uploadsPath);
            }

            // Generate unique file name
            var fileExtension = Path.GetExtension(uploadMediaDto.File.FileName);
            var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
            var filePath = Path.Combine(uploadsPath, uniqueFileName);

            // Save file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await uploadMediaDto.File.CopyToAsync(stream);
            }

            // Create media file entity
            var mediaFile = new MediaFile
            {
                Id = Guid.NewGuid().ToString(),
                FileName = uploadMediaDto.File.FileName,
                MimeType = uploadMediaDto.File.ContentType,
                Url = $"/{UploadsFolder}/{uniqueFileName}",
                Size = uploadMediaDto.File.Length,
                UploadedBy = userId,
                CreatedOn = DateTime.UtcNow,
                IsDeleted = false
            };

            _context.MediaFiles.Add(mediaFile);
            await _context.SaveChangesAsync();

            var mediaFileDto = new MediaFileDto
            {
                Id = mediaFile.Id,
                FileName = mediaFile.FileName,
                MimeType = mediaFile.MimeType,
                Url = mediaFile.Url,
                Size = mediaFile.Size,
                UploadedBy = mediaFile.UploadedBy,
                CreatedAt = mediaFile.CreatedOn
            };

            return CreatedAtAction(nameof(GetMediaFile), new { id = mediaFileDto.Id }, mediaFileDto);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")] // Only Admin can delete media files
        public async Task<IActionResult> DeleteMedia(string id)
        {
            var mediaFile = await _context.MediaFiles.FindAsync(id);

            if (mediaFile == null || mediaFile.IsDeleted)
            {
                return NotFound("Media file not found.");
            }

            // Soft delete
            mediaFile.IsDeleted = true;
            mediaFile.UpdatedOn = DateTime.UtcNow;

            // Optionally delete physical file
            if (!string.IsNullOrEmpty(mediaFile.Url))
            {
                var filePath = Path.Combine(_environment.WebRootPath ?? _environment.ContentRootPath, mediaFile.Url.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                {
                    try
                    {
                        System.IO.File.Delete(filePath);
                    }
                    catch
                    {
                        // Log error but continue with soft delete
                    }
                }
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}