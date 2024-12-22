using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace Security.Models.BEL
{
    public class UserInRoleBEL
    {
        public string EmpID { get; set; }
        public string EmpName { get; set; }
        public string UserID { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string RoleID { get; set; }
        public string RoleName { get; set; }  
        public bool IsActive { get;set; }
      //  public string YetAssigned { get; set; }
    }
}