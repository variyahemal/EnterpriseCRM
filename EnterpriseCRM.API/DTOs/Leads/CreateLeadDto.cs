using System.ComponentModel.DataAnnotations;

namespace EnterpriseCRM.API.DTOs.Leads
{
    public class CreateLeadDto
    {
        [Required]
        [StringLength(100)]
        public string? FirstName { get; set; }

        [Required]
        [StringLength(100)]
        public string? LastName { get; set; }

        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Phone]
        public string? Phone { get; set; }

        [StringLength(200)]
        public string? Company { get; set; }

        [StringLength(100)]
        public string? Id { get; set; }

        [StringLength(100)]
        public string? Source { get; set; }
    }
}