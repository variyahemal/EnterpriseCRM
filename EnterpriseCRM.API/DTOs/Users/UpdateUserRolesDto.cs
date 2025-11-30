using System.Collections.Generic;

namespace EnterpriseCRM.API.DTOs.Users
{
    public class UpdateUserRolesDto
    {
        public List<string>? Roles { get; set; }
    }
}