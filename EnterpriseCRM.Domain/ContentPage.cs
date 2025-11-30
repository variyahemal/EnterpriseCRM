using System;

namespace EnterpriseCRM.Domain
{
    public class ContentPage : BaseEntity<string>
    {
        public string Title { get; set; }
        public string Slug { get; set; }
        public string Template { get; set; }
        public string HTMLContent { get; set; }
    }
}