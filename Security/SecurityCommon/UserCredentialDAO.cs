using System.Data;

namespace Security.WorkflowCommon
{
    public class UserCredentialDAO
    {
        DBConnection dbConn = new DBConnection();
        DBHelper dbHelper = new DBHelper();
        public List<UserCredentialDetailsBEO> CheckUserCredential()
        {

            string Qry = @" SELECT ur.UserID,ur.RoleID,r.RoleName,ur.EmpID,dbHR.dbo.GetName(ur.EmpID,'ER') EmpName,dbHR.dbo.GetName(ur.EmpID,'EE') SupervisorID,dbHR.dbo.GetName(dbHR.dbo.GetName(ur.EmpID,'EE'),'ER') SupervisorName,dbHR.dbo.GetName(ur.EmpID,'EE d') Designation,dbHR.dbo.GetName(ur.EmpID,'EE ed') EmploymentDate,u.NewPassword,u.OldPassword,ur.IsActive 
                            FROM Sa_UserInRole as ur, Sa_UserCredential as u,Sa_Role r Where ur.UserID=u.UserID and ur.RoleID=r.RoleID and ur.IsActive='1' ";
            DataTable dt = dbHelper.GetDataTable(dbConn.SAConnStrReader(), Qry);
            List<UserCredentialDetailsBEO> item;
            item = (from DataRow row in dt.Rows
                    select new UserCredentialDetailsBEO
                    {
                        UserName = row["UserID"].ToString(),
                        Password = row["NewPassword"].ToString(),
                        RoleId = row["RoleID"].ToString(),
                        RoleName = row["RoleName"].ToString(),
                        EmployeeId = row["EmpID"].ToString(),
                        EmployeeName = row["EmpName"].ToString(),
                        Designation = row["Designation"].ToString(),
                        JoiningDate = row["EmploymentDate"].ToString(),

                    }).ToList();
            return item;



        }
    }
}
