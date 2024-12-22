using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;


namespace Security.Models.DAL
{
    public class UserInRoleDAO : ReturnData
    {
        DBConnection  dbConn=new DBConnection();
        DBHelper dbHelper = new DBHelper();
        public bool SaveUpdate(UserInRoleBEL master)
        {
            try
            {
                bool isTrue = false;
                string Str = master.EmpID;  // master.EmpID.Replace("[", "").Replace("]", "").Replace("\"", "");
                String[] SubStr = Str.Split(',');
                 for (int i = 0; i < SubStr.Length; i++)
                 {
                     string d = SubStr[i].ToString();
                     string Qry = "Sa_UserInRoleSP @RoleID='" + master.RoleID.Trim() + "', @UserID='" + master.UserID.Trim() + "',@NewPassword='" + master.Password.Trim() + "',@EmpID='" + master.EmpID.Trim() + "',@IsActive='" + (master.IsActive == false ? '0' : '1') + "'";
                     var tuple = dbHelper.CmdExecuteGetMultipleData(dbConn.SAConnStrReader(), Qry);
                     if (tuple.Item1)
                     {
                         MaxID = tuple.Item2;
                         IUMode = tuple.Item3;
                         isTrue= true;

                     }
                     else
                     {
                         isTrue= false;
                     }
                 }
                 return isTrue;
            }
            catch (Exception errorException)
            {
                throw errorException;
            }
        }

        public List<UserInRoleBEL> GetUserInRoleList(string sessionRoleId)
        {
            string Qry = @"SELECT ur.UserID,ur.RoleID,r.RoleName,ur.EmpID,dbHR.dbo.GetName(ur.EmpID,'ER') EmpName,u.NewPassword,u.OldPassword,ur.IsActive 
                           FROM Sa_UserInRole as ur, Sa_UserCredential as u,Sa_Role r 
                           Where ur.UserID=u.UserID and ur.RoleID=r.RoleID AND ur.RoleID>='"+ sessionRoleId+ "' Order By EmpID";
            DataTable dt = dbHelper.GetDataTable(dbConn.SAConnStrReader(), Qry);
            List<UserInRoleBEL> item;

            item = (from DataRow row in dt.Rows
                    select new UserInRoleBEL
                    {
                        RoleID = row["RoleID"].ToString(),
                        RoleName = row["RoleName"].ToString(),
                        UserID = row["UserID"].ToString(),                     
                        EmpID = row["EmpID"].ToString().Trim(),
                        EmpName = row["EmpName"].ToString().Trim(),
                        Password = row["NewPassword"].ToString(),
                        ConfirmPassword = row["NewPassword"].ToString(),
                        IsActive = Convert.ToBoolean(row["IsActive"].ToString())

                    }).ToList();
            return item;
        }

        public List<UserInRoleBEL> GetEmployeeList()
        {
            string Qry = "Select EmpID,EmpName From HR_Emp_Recruitment where IsActive=1";
            DataTable dt = dbHelper.GetDataTable(dbConn.HRConnStrReader(), Qry);
            List<UserInRoleBEL> item;

            item = (from DataRow row in dt.Rows
                    select new UserInRoleBEL
                    {
                        EmpID = row["EmpID"].ToString().Trim(),
                        EmpName = row["EmpName"].ToString().Trim()


                    }).ToList();
            return item;
        }

       public List<UserInRoleBEL> GetUserList()
        {
            string Qry = "Select UserID,UserID,EmpID,dbHR.dbo.GetName(EmpID,'ER') EmpName From Sa_UserInRole Where IsActive=1";
            DataTable dt = dbHelper.GetDataTable(dbConn.SAConnStrReader(), Qry);
            List<UserInRoleBEL> item;

            item = (from DataRow row in dt.Rows
                    select new UserInRoleBEL
                    {
                        UserID = row["UserID"].ToString(),
                        EmpName = row["EmpName"].ToString()


                    }).ToList();
            return item;
        }

       public List<UserInRoleBEL> GetEmpYetAssignedList(string RoleID)
       {
            //string Qry = "  select a.EmpID,a.EmpName,a.YetAssigned from "+
            //            " (Select EmpID,dbHR.dbo.GetName(EmpID,'ER') EmpName,'true' YetAssigned from Sa_UserInRole Where RoleID='" + RoleID + "' and IsActive=1 and EmpID is not null and  EmpID!='' union all" +
            //            " Select EmpID,EmpName,'false' YetAssigned From dbHR.dbo.HR_Emp_Recruitment where IsActive=1  and EmpID not in (Select EmpID from Sa_UserInRole Where  IsActive=1)) a" +
            //            " Order by  CASE WHEN Upper(a.YetAssigned) = upper('true') THEN 1 ELSE 2 END, a.YetAssigned,a.EmpName ";

            string Qry = "Select EmpID,EmpName from dbHR.dbo.HR_Emp_Recruitment";
                     

            DataTable dt = dbHelper.GetDataTable(dbConn.SAConnStrReader(), Qry);
           List<UserInRoleBEL> item;

           item = (from DataRow row in dt.Rows
                   select new UserInRoleBEL
                   {
                       EmpID = row["EmpID"].ToString().Trim(),
                       EmpName = row["EmpName"].ToString().Trim(),
                      // YetAssigned = row["YetAssigned"].ToString()

                   }).ToList();
           return item;
       }
    }
}