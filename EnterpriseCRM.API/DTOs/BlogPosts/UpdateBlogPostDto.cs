using System.ComponentModel.DataAnnotations;

namespace EnterpriseCRM.API.DTOs.BlogPosts
{
    public class UpdateBlogPostDto
    {
        [Required]
        [StringLength(200)]
        public string? Title { get; set; }

        [Required]
        public string? Content { get; set; }
    }
}