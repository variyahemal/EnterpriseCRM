using System;
using System.Collections.Generic;

namespace EnterpriseCRM.Domain
{
    public class Category : BaseEntity<string>
    {
        public string Name { get; set; }
        public string Slug { get; set; }

        // Navigation properties
        public ICollection<BlogPostCategory> BlogPostCategories { get; set; }
    }
}