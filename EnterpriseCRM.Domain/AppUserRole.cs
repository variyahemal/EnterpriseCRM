using System;

namespace EnterpriseCRM.Domain
{
    public class AppUserRole : BaseEntity<string>
    {
        public string UserId { get; set; }
        public string RoleId { get; set; }

        public AppUser User { get; set; }
        public AppRole Role { get; set; }
    }
}