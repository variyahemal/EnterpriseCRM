using Microsoft.AspNetCore.Http;

namespace EnterpriseCRM.API.DTOs.Media
{
    public class UploadMediaDto
    {
        public IFormFile? File { get; set; }
    }
}