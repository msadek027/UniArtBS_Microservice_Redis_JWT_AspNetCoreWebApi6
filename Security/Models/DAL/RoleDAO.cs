using Security.Models.BEL;
using Security.WorkflowCommon;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;


namespace Security.Models.DAL
{
    public class RoleDAO :ReturnData
    {
        DBConnection dbConn = new DBConnection();
        DBHelper dbHelper = new DBHelper();

        public Boolean SaveUpdate(RoleBEL roleBEL)
        {
            try
            {
                string Qry = "Sa_RoleSP @ID='" + roleBEL.RoleID + "',@Name='" + roleBEL.RoleName + "',@IsActive='" + (roleBEL.IsActive == false ? '0' : '1') + "'";
                var tuple = dbHelper.CmdExecuteGetMultipleData(dbConn.SAConnStrReader(), Qry);
                if (tuple.Item1)
                {
                    MaxID = tuple.Item2;
                    IUMode = tuple.Item3;
                    return true;

                }


                else
                {
                    return false;
                }

            }
            catch (Exception errorException)
            {
                throw errorException;
            }
        }

        public bool DeleteExecute(RoleBEL roleBEL)
        {
            string Qry = " Delete from Sa_Role Where RoleID='" + roleBEL.RoleID + "'";
            if (dbHelper.CmdExecute(dbConn.SAConnStrReader(), Qry))
            {
                return true;
            }
            else
            {
                return false;
            }
        }





        public List<RoleBEL> GetRoleList()
        {
            string Qry = "Select RoleID,RoleName,IsActive From Sa_Role ";
            DataTable dt = dbHelper.GetDataTable(dbConn.SAConnStrReader(), Qry);
            List<RoleBEL> item;

            item = (from DataRow row in dt.Rows
                    select new RoleBEL
                    {
                        RoleID = row["RoleID"].ToString(),
                        RoleName = row["RoleName"].ToString(),
                        IsActive =Convert.ToBoolean(row["IsActive"].ToString())

                    }).ToList();
            return item;
        }
        public DataTable GetDataTableRole()
        {
            string Qry = "Select RoleID,RoleName,IsActive From Sa_Role ";
            DataTable dt = dbHelper.GetDataTable(dbConn.SAConnStrReader(), Qry);
           
            return dt;
        }
    }
}