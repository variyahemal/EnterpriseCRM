using System;

namespace EnterpriseCRM.Domain
{
    public class MediaFile : BaseEntity<string>
    {
        public string FileName { get; set; }
        public string MimeType { get; set; }
        public string Url { get; set; }
        public long Size { get; set; }
        public string UploadedBy { get; set; }

        // Navigation properties
        public AppUser Uploader { get; set; }
    }
}