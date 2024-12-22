using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Security.Models.BEL
{
    public class RoleInSoftwareModuleBEL
    {        
        public string RoleID { get; set; }
        public string SoftwareID { get; set; }
        public string SoftwareFullName { get; set; }
        public string SoftwareShortName { get; set; }
        public string ModuleID { get; set; }
        public string ModuleName { get; set; }
        public Boolean IsActive { get; set; }
        public virtual ICollection<RoleInSoftwareModuleBEL> detailsList { get; set; }
  

  
    }
}