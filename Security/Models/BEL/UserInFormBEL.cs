using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Security.Models.BEL
{
    public class UserInFormBEL
    {
        public string UserID { get; set; }      
        public string RoleID { get; set; }
        public string RoleName { get; set; }
        public string SoftwareID { get; set; }
        public string SoftwareShortName { get; set; }
        public string ModuleID { get; set; }
        public string ModuleName { get; set; }
        public string FormID { get; set; }
        public string FormName { get; set; }
        public string FormURL { get; set; }
        public Boolean ViewPermission { get; set; }
        public Boolean SavePermission { get; set; }
        public Boolean EditPermission { get; set; }
        public Boolean DeletePermission { get; set; }
        public Boolean PrintPermission { get; set; }

        public virtual ICollection<UserInFormBEL> detailsList { get; set; }
    }
}