using System.ComponentModel.DataAnnotations;

namespace EnterpriseCRM.API.DTOs.Leads
{
    public class UpdateLeadStatusDto
    {
        public string? Status { get; set; }
    }
}