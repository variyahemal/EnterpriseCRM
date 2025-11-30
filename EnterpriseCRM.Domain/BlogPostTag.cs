using System;

namespace EnterpriseCRM.Domain
{
    public class BlogPostTag : BaseEntity<string>
    {
        public Guid BlogPostId { get; set; }
        public string TagId { get; set; }

        public BlogPost BlogPost { get; set; }
        public Tag Tag { get; set; }
    }
}