using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreatePermissions
{
    class SPDOGroup
    {
        public string GroupName { get; set; }
        public string Description { get; set; }
    }

    class SPDOPermission
    {
        public SPBasePermissions Permissions { get; set; }
        public string RoleName { get; set; }
        public string RoleDescription { get; set; }
    }
}
