using System;
using System.ComponentModel.DataAnnotations;

namespace EnterpriseCRM.API.DTOs.BlogPosts
{
    public class BlogPostDto
    {
        public string? Id { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public string? AuthorId { get; set; }
        public string? AuthorName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsPublished { get; set; }
        public DateTime? PublishedAt { get; set; }
    }
}