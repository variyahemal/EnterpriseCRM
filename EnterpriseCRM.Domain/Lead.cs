using System;
using System.Collections.Generic;

namespace EnterpriseCRM.Domain
{
    public class Lead : BaseEntity<Guid>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Company { get; set; }
        public string Source { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public string? AssignedUserId { get; set; }

        // Navigation properties
        public AppUser AssignedUser { get; set; }
        public ICollection<Contact> Contacts { get; set; }
    }
}