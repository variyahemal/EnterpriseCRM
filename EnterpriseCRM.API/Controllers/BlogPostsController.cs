using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EnterpriseCRM.API.DTOs.BlogPosts;
using EnterpriseCRM.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using EnterpriseCRM.Infrastructure.Persistence;

namespace EnterpriseCRM.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/blogposts")]
    [Authorize(Roles = "Admin,Editor")] // Base authorization for BlogPostsController
    public class BlogPostsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public BlogPostsController(ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet("public")]
        [AllowAnonymous] // Publicly accessible endpoint
        public async Task<ActionResult<IEnumerable<BlogPostDto>>> GetPublicBlogPosts(
            [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (page < 1 || pageSize < 1)
            {
                return BadRequest("Page and pageSize must be positive integers.");
            }

            var blogPosts = await _context.BlogPosts
                .Where(bp => bp.IsPublished && !bp.IsDeleted)
                .OrderByDescending(bp => bp.PublishedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(bp => new BlogPostDto
                {
                    Id = bp.Id.ToString(),
                    Title = bp.Title,
                    Content = bp.Content,
                    AuthorId = bp.AuthorId ?? string.Empty,
                    AuthorName = _context.Users.Where(u => u.Id == bp.AuthorId).Select(u => u.FirstName + " " + u.LastName).FirstOrDefault(),
                    CreatedAt = bp.CreatedAt,
                    UpdatedAt = bp.UpdatedAt,
                    IsPublished = bp.IsPublished,
                    PublishedAt = bp.PublishedAt
                })
                .ToListAsync();

            return Ok(blogPosts);
        }

        [HttpGet("admin")]
        [Authorize(Roles = "Admin,Editor")]
        public async Task<ActionResult<IEnumerable<BlogPostDto>>> GetAdminBlogPosts(
            [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (page < 1 || pageSize < 1)
            {
                return BadRequest("Page and pageSize must be positive integers.");
            }

            var blogPosts = await _context.BlogPosts
                .Where(bp => !bp.IsDeleted)
                .OrderByDescending(bp => bp.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(bp => new BlogPostDto
                {
                    Id = bp.Id.ToString(),
                    Title = bp.Title,
                    Content = bp.Content,
                    AuthorId = bp.AuthorId ?? string.Empty,
                    AuthorName = _context.Users.Where(u => u.Id == bp.AuthorId).Select(u => u.FirstName + " " + u.LastName).FirstOrDefault(),
                    CreatedAt = bp.CreatedAt,
                    UpdatedAt = bp.UpdatedAt,
                    IsPublished = bp.IsPublished,
                    PublishedAt = bp.PublishedAt
                })
                .ToListAsync();

            return Ok(blogPosts);
        }

        [HttpGet("{id}")]
        [AllowAnonymous] // Accessible publicly if published, or by Admin/Editor if not
        public async Task<ActionResult<BlogPostDto>> GetBlogPost(string id)
        {
            if (!Guid.TryParse(id, out var blogPostId))
            {
                return BadRequest("Invalid blog post ID format.");
            }

            var blogPost = await _context.BlogPosts
                .Where(bp => bp.Id == blogPostId && !bp.IsDeleted)
                .Select(bp => new BlogPostDto
                {
                    Id = bp.Id.ToString(),
                    Title = bp.Title,
                    Content = bp.Content,
                    AuthorId = bp.AuthorId ?? string.Empty,
                    AuthorName = _context.Users.Where(u => u.Id == bp.AuthorId).Select(u => u.FirstName + " " + u.LastName).FirstOrDefault(),
                    CreatedAt = bp.CreatedAt,
                    UpdatedAt = bp.UpdatedAt,
                    IsPublished = bp.IsPublished,
                    PublishedAt = bp.PublishedAt
                })
                .FirstOrDefaultAsync();

            if (blogPost == null)
            {
                return NotFound("Blog post not found.");
            }

            // If not published, only Admin/Editor can view
            if (!blogPost.IsPublished && !(User.IsInRole("Admin") || User.IsInRole("Editor")))
            {
                return Forbid();
            }

            return Ok(blogPost);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Editor")]
        public async Task<ActionResult<BlogPostDto>> CreateBlogPost([FromBody] CreateBlogPostDto createBlogPostDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized("User is not authenticated.");
            }

            var blogPost = new BlogPost
            {
                Id = Guid.NewGuid(),
                Title = createBlogPostDto.Title ?? string.Empty,
                Content = createBlogPostDto.Content ?? string.Empty,
                AuthorId = userId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsPublished = false // New posts are not published by default
            };

            _context.BlogPosts.Add(blogPost);
            await _context.SaveChangesAsync();

            var authorName = _context.Users.Where(u => u.Id == userId).Select(u => u.FirstName + " " + u.LastName).FirstOrDefault();

            var blogPostDto = new BlogPostDto
            {
                Id = blogPost.Id.ToString(),
                Title = blogPost.Title,
                Content = blogPost.Content,
                AuthorId = blogPost.AuthorId,
                AuthorName = authorName,
                CreatedAt = blogPost.CreatedAt,
                UpdatedAt = blogPost.UpdatedAt,
                IsPublished = blogPost.IsPublished,
                PublishedAt = blogPost.PublishedAt
            };

            return CreatedAtAction(nameof(GetBlogPost), new { id = blogPostDto.Id }, blogPostDto);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Editor")]
        public async Task<ActionResult<BlogPostDto>> UpdateBlogPost(string id, [FromBody] UpdateBlogPostDto updateBlogPostDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!Guid.TryParse(id, out var blogPostId))
            {
                return BadRequest("Invalid blog post ID format.");
            }

            var blogPost = await _context.BlogPosts.FindAsync(blogPostId);

            if (blogPost == null)
            {
                return NotFound("Blog post not found.");
            }

            blogPost.Title = updateBlogPostDto.Title ?? blogPost.Title;
            blogPost.Content = updateBlogPostDto.Content ?? blogPost.Content;
            blogPost.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            var authorName = _context.Users.Where(u => u.Id == blogPost.AuthorId).Select(u => u.FirstName + " " + u.LastName).FirstOrDefault();

            var blogPostDto = new BlogPostDto
            {
                Id = blogPost.Id.ToString(),
                Title = blogPost.Title,
                Content = blogPost.Content,
                AuthorId = blogPost.AuthorId,
                AuthorName = authorName,
                CreatedAt = blogPost.CreatedAt,
                UpdatedAt = blogPost.UpdatedAt,
                IsPublished = blogPost.IsPublished,
                PublishedAt = blogPost.PublishedAt
            };

            return Ok(blogPostDto);
        }

        [HttpPut("{id}/publish")]
        [Authorize(Roles = "Admin,Editor")]
        public async Task<ActionResult<BlogPostDto>> PublishBlogPost(string id, [FromQuery] bool publish)
        {
            if (!Guid.TryParse(id, out var blogPostId))
            {
                return BadRequest("Invalid blog post ID format.");
            }

            var blogPost = await _context.BlogPosts.FindAsync(blogPostId);

            if (blogPost == null)
            {
                return NotFound("Blog post not found.");
            }

            blogPost.IsPublished = publish;
            blogPost.UpdatedAt = DateTime.UtcNow;
            if (publish)
            {
                blogPost.PublishedAt = DateTime.UtcNow;
            }
            else
            {
                blogPost.PublishedAt = null;
            }

            await _context.SaveChangesAsync();

            var authorName = _context.Users.Where(u => u.Id == blogPost.AuthorId).Select(u => u.FirstName + " " + u.LastName).FirstOrDefault();

            var blogPostDto = new BlogPostDto
            {
                Id = blogPost.Id.ToString(),
                Title = blogPost.Title,
                Content = blogPost.Content,
                AuthorId = blogPost.AuthorId,
                AuthorName = authorName,
                CreatedAt = blogPost.CreatedAt,
                UpdatedAt = blogPost.UpdatedAt,
                IsPublished = blogPost.IsPublished,
                PublishedAt = blogPost.PublishedAt
            };

            return Ok(blogPostDto);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")] // Only Admin can soft delete blog posts
        public async Task<ActionResult> DeleteBlogPost(string id)
        {
            if (!Guid.TryParse(id, out var blogPostId))
            {
                return BadRequest("Invalid blog post ID format.");
            }

            var blogPost = await _context.BlogPosts.FindAsync(blogPostId);

            if (blogPost == null)
            {
                return NotFound("Blog post not found.");
            }

            if (blogPost.IsDeleted)
            {
                return Conflict("Blog post already deleted.");
            }

            blogPost.IsDeleted = true;
            blogPost.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}