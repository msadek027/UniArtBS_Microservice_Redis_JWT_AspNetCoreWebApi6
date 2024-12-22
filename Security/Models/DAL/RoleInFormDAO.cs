using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;



namespace Security.Models.DAL
{
    public class RoleInFormDAO : ReturnData 
    {
        DBConnection dbConn = new DBConnection();
        DBHelper dbHelper=new DBHelper();
        public List<RoleInFormBEL> GetRoleInFormPermissionList(string RoleID)
        {


            //HttpContext.Current.Session["RoleID"].ToString()

            RoleID = (RoleID == null || RoleID == "") ? RoleID : RoleID;

            string Qry = "Sa_RoleInFormPermissionMappingSP @RoleID='" + RoleID.Trim() + "'";


            DataTable dt = dbHelper.GetDataTable(dbConn.SAConnStrReader(),Qry);
            List<RoleInFormBEL> item;
            //using lamdaexpression
            item = (from DataRow row in dt.Rows
                    select new RoleInFormBEL
                    {
                        SoftwareID = row["SoftwareID"].ToString(),
                        SoftwareShortName = row["SoftwareShortName"].ToString(),
                        ModuleID = row["ModuleID"].ToString(),
                        ModuleName = row["ModuleName"].ToString(),
                        FormID = row["FormID"].ToString(),
                        FormName = row["FormName"].ToString(),
                        FormURL = row["FormURL"].ToString(),
                        ViewPermission = Convert.ToBoolean(row["ViewPermission"].ToString()),
                        SavePermission = Convert.ToBoolean(row["SavePermission"].ToString()),
                        EditPermission = Convert.ToBoolean(row["EditPermission"].ToString()),
                        DeletePermission = Convert.ToBoolean(row["DeletePermission"].ToString()),
                        PrintPermission = Convert.ToBoolean(row["PrintPermission"].ToString()),



                    }).ToList();

            return item;
        }

        public bool SaveUpdate(RoleInFormBEL master)
        {//HttpContext.Current.Session["UserID"].ToString()
            string UserID = "";
            bool IsTrue = false;
            if (master != null)
            {
                if (master.detailsList != null)
                {
                    foreach (RoleInFormBEL details in master.detailsList)
                    {
                        IsTrue = false;
                        string VSEDP = Convert.ToString(details.ViewPermission == true ? 1 : 0) + Convert.ToString(details.SavePermission == true ? 1 : 0) + Convert.ToString(details.EditPermission == true ? 1 : 0) + Convert.ToString(details.DeletePermission == true ? 1 : 0) + Convert.ToString(details.PrintPermission == true ? 1 : 0);

                        string Qry = "Sa_RoleInFormPermissionSP @RoleSoftwareModuleFormID='" + details.RoleID + details.SoftwareID + details.ModuleID + details.FormID + "', @VSEDP='" + VSEDP + "',@SetOn='" + UserID + "'";
                        var tuple = dbHelper.CmdExecuteGetMultipleData(dbConn.SAConnStrReader(), Qry);
                        if (tuple.Item1)
                        {
                            MaxID = tuple.Item2;
                            IUMode = tuple.Item3;
                            if (dbHelper.CmdExecute(dbConn.SAConnStrReader(), "Sa_MenuSP @pUserID='" + master.UserID + "'"))
                            {
                                IsTrue = true;
                            }
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