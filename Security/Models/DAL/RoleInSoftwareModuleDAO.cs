using Security.Models.BEL;
using Security.WorkflowCommon;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;


namespace Security.Models.DAL
{
    public class RoleInSoftwareModuleDAO:ReturnData
    {
        DBConnection dbConn = new DBConnection();
        DBHelper dbHelper = new DBHelper();
        public List<RoleInSoftwareModuleBEL> GetRoleInSoftwareModuleMappingList(string RoleID)
        {
            string Qry = "Sa_RoleInSoftwareModuleMappingSP @RoleID='" + RoleID.Trim() + "'";


            DataTable dt = dbHelper.GetDataTable(dbConn.SAConnStrReader(), Qry);
            List<RoleInSoftwareModuleBEL> item;
            //using lamdaexpression
            item = (from DataRow row in dt.Rows
                    select new RoleInSoftwareModuleBEL
                    {
                       
                        SoftwareID = row["SoftwareID"].ToString(),
                        SoftwareShortName = row["SoftwareShortName"].ToString(),
                        SoftwareFullName = row["SoftwareFullName"].ToString(),
                        ModuleID = row["ModuleID"].ToString(),
                        ModuleName = row["ModuleName"].ToString(),
                        IsActive = Convert.ToBoolean(row["IsActive"].ToString())



                    }).ToList();

            return item;
        }


        public Boolean SaveUpdate(RoleInSoftwareModuleBEL master)
        {
            bool IsTrue = false;
            if (master != null)
            {
                if (master.detailsList != null)
                {
                    foreach (RoleInSoftwareModuleBEL details in master.detailsList)
                    {
                        IsTrue = false;
                        string Qry = "Sa_RoleInSoftwareModuleSP @RoleSoftwareModuleID='" + details.RoleID + details.SoftwareID + details.ModuleID + "',@IsActive='" + (details.IsActive == false ? '0' : '1') + "'";
                        var tuple = dbHelper.CmdExecuteGetMultipleData(dbConn.SAConnStrReader(), Qry);
                        if (tuple.Item1)
                        {
                            MaxID = tuple.Item2;
                            IUMode = tuple.Item3;
                            IsTrue = true;
                        }
                        else
                        {
                            IsTrue = false;
                        }
                    }
                }
            }
            return IsTrue;
            }
           
        


    }
}