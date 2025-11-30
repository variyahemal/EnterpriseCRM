using System;
using System.Collections.Generic;

namespace EnterpriseCRM.Domain
{
    public class Contact : BaseEntity<Guid>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Company { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Country { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public Guid? LeadId { get; set; }
        public string? OwnerId { get; set; }

        // Navigation properties
        public Lead Lead { get; set; }
        public AppUser Owner { get; set; }
        public ICollection<FollowUp> FollowUps { get; set; }
        public ICollection<SaleOpportunity> SaleOpportunities { get; set; }
    }
}