using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Workflow.Models.BEL;
using Workflow.WorkflowCommon;


namespace Workflow.Models.DAL
{
    public class PermissionSetupDAO
    {
        DBConnection dbConn = new DBConnection();
        DBHelper dbHelper = new DBHelper();
        TerminalLogger terminal = new TerminalLogger();
        internal object GetUserList(string RoleId, string UserId,string WorkflowId)
        {
            List<PermissionSetupBEO> list = null;
            string Qry = @"Select * from(Select A.StageId,StageName,StageSL,A.WorkflowId,B.WorkflowName,C.RoleId,C.UserId,C.IsActive,
                        CASE When C.IsActive=0  Then 'False' When C.IsActive=1 AND ISNULL(C.RoleId,'')='' Then 'False' Else 'True' END RoleStatus,
                        CASE When C.IsActive=0  Then 'False' When  C.IsActive=1 AND ISNULL(C.UserId,'')='' Then 'False' Else 'True' END UserStatus 
                        from WFM_StageSetup As A Inner Join WFM_WorkflowCreation As B ON A.WorkflowId=B.WorkflowId
                        Left Join WFM_StageSetup_UserPermission As C On A.WorkflowId=C.WorkflowId And A.StageId=C.StageId) As T Where 1=1";

            if (WorkflowId != null && WorkflowId != "")
            {
                Qry = Qry + " And WorkflowId='" + WorkflowId + "'";
            }
            //if (RoleId != null && RoleId != "")
            //{
            //    Qry = Qry + " And RoleId='" + RoleId + "'";
            //}
            //if (UserId != null && UserId != "")
            //{
            //    Qry = Qry + " And UserId='" + UserId + "'";
            //}
       
            DataTable dt = dbHelper.GetDataTable(dbConn.DocConnStrReader(), Qry);


            list = (from DataRow row in dt.Rows
                    select new PermissionSetupBEO
                    {
                        WorkflowId = row["WorkflowId"].ToString(),
                        WorkflowName = row["WorkflowName"].ToString(),
                        StageSL = row["StageSL"].ToString(),
                        StageId = row["StageId"].ToString(),
                        StageName = row["StageName"].ToString(),
                        IsActive = (RoleId !=null && UserId!=null)?Convert.ToBoolean(row["UserStatus"].ToString()): Convert.ToBoolean(row["RoleStatus"].ToString()),
                       

                    }).ToList();
            return list;
        }

        internal bool SaveUpdate(string RoleId,string UserId, string WorkflowId,List<PermissionSetupBEO> stageList)
        {
            bool isTrue = false;
        
            try
            {
                if (RoleId != null && RoleId != "" && UserId != null && UserId != "" )
                {
                    string QryDel = "Delete from WFM_StageSetup_UserPermission Where RoleId='" + RoleId + "' and UserId='" + UserId + "'";
                    if (WorkflowId != null && WorkflowId != "")
                    {
                        QryDel = QryDel + " And WorkflowId='" + WorkflowId + "'";
                    }
                    dbHelper.CmdExecute(dbConn.DocConnStrReader(), QryDel);
                    foreach (PermissionSetupBEO model in stageList)
                    {
                        isTrue = false;

                        string Qry = @"Insert Into WFM_StageSetup_UserPermission(StageId,WorkflowId,RoleId,UserId,IsActive) 
                              Values('" + model.StageId + "','" + model.WorkflowId + "','" + RoleId + "','" + UserId + "'," + (model.IsActive == false ? '0' : '1') + ")";

                        if (dbHelper.CmdExecute(dbConn.DocConnStrReader(), Qry))
                        {
                            isTrue = true;
                        }
                    }
                }
                return isTrue;
            }
            catch (Exception errorException)
            {
                throw errorException;
            }
        }
    }
}