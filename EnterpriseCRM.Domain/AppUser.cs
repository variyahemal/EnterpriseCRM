using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace EnterpriseCRM.Domain
{
    public class AppUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }

        // Navigation properties
        public ICollection<AppUserRole> UserRoles { get; set; }
        public ICollection<Contact> Contacts { get; set; }
        public ICollection<Lead> AssignedLeads { get; set; }
        public ICollection<SaleOpportunity> SaleOpportunities { get; set; }
        public ICollection<BlogPost> BlogPosts { get; set; }
        public ICollection<MediaFile> MediaFiles { get; set; }
        public ICollection<AuditLog> AuditLogs { get; set; }
    }
}