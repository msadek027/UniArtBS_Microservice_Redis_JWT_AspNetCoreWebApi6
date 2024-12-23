#region ***** Author Information *****
/*  ------ Author & Class Description ------------
 * Created by       : Engr. Mir sadequr Rahman[Sadek](engr.msadek027@gmail.com)
 * Create Date      : 27/04/2014
 * Modify by & Date : Mir & 27/04/2014
 * Description      : (Security Module) user all security check function
 */
#endregion 
using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections;

namespace Security.Models.BEL
{
    public class RoleBEL
    {
        public string RoleID { get; set; }
        public string RoleName { get; set; }
        public Boolean IsActive { get; set; }            
       
       
    }
}