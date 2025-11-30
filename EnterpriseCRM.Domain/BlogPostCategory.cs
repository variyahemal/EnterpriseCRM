using System;

namespace EnterpriseCRM.Domain
{
    public class BlogPostCategory : BaseEntity<string>
    {
        public Guid BlogPostId { get; set; }
        public string CategoryId { get; set; }

        public BlogPost? BlogPost { get; set; }
        public Category? Category { get; set; }
    }
}