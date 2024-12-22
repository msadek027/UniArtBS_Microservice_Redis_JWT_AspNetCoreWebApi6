#region ***** Author Information *****  
/* Auto Generated Code by Designer 1.0, Created Date2/13/2009. 
 *  Created by :               . 
 *   Modify by :               . 
 *  Modify date:               . */
#endregion 
using System;
using System.Data;
#region ***** Author Information *****
/*  ------ Author & Class Description ------------
 * Created by       : Engr. Mir sadequr Rahman[Sadek](engr.msadek027@gmail.com)
 * Create Date      : 27/04/2014
 * Modify by & Date : Mir & 27/04/2014
 * Description      : (Security Module) user all security check function
 */
#endregion 

using System.Data.SqlClient;
using System.Collections;

namespace Security.Models.BEL
{
    public class UserBEL:GlobalBEL
    {

        public String UserID { get; set; }
        public String UserPassword { get; set; }
        public String EmpID { get; set; }
        public String EmpName { get; set; }
        public String RoleID { get; set; }
        public String RoleName { get; set; }
        public Boolean Status { get; set; }
    

       
       
      
       
    }
}