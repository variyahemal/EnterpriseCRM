using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace EnterpriseCRM.Domain
{
    public class AppRole : IdentityRole
    {
        public string Name { get; set; }
        public string NormalizedName { get; set; }
        public string ConcurrencyStamp { get; set; }

        // Navigation properties
        public ICollection<AppUserRole> UserRoles { get; set; }
    }
}