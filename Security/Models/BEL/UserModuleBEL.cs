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
    public class UserModuleBEL
    {
        #region ***** Private variavle *****
        private String _sysid = "";
        private String _userid;
        private String _moduleid;

        #endregion

        #region ***** Public properties *****
        public String SysID
        {
            get { return _sysid; }
            set { _sysid = value; }
        }
        public String UserID
        {
            get { return _userid; }
            set { _userid = value; }
        }
        public String ModuleID
        {
            get { return _moduleid; }
            set { _moduleid = value; }
        }
        #endregion
    }
}