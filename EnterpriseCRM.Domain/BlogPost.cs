using System;
using System.Collections.Generic;

namespace EnterpriseCRM.Domain
{
    public class BlogPost : BaseEntity<Guid>
    {
        public string? Title { get; set; }
        public string? Content { get; set; }
        public string? AuthorId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsPublished { get; set; }
        public DateTime? PublishedAt { get; set; }
        public bool IsDeleted { get; set; }

        // Navigation properties
        public AppUser? Author { get; set; }
        public ICollection<BlogPostCategory>? BlogPostCategories { get; set; }
        public ICollection<BlogPostTag>? BlogPostTags { get; set; }
    }
}