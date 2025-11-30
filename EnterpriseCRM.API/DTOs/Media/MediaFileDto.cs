namespace EnterpriseCRM.API.DTOs.Media
{
    public class MediaFileDto
    {
        public string Id { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string MimeType { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public long Size { get; set; }
        public string UploadedBy { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}

