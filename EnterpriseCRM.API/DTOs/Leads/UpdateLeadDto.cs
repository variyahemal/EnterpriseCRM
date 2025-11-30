using System.ComponentModel.DataAnnotations;

namespace EnterpriseCRM.API.DTOs.Leads
{
    public class UpdateLeadDto
    {
        [StringLength(100)]
        public string? FirstName { get; set; }

        [StringLength(100)]
        public string? LastName { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        [Phone]
        public string? Phone { get; set; }

        [StringLength(200)]
        public string? Company { get; set; }

        [StringLength(100)]
        public string? Source { get; set; }

        [StringLength(50)]
        public string? Status { get; set; }
    }
}

