using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Workflow.Models.BEL;
using Workflow.WorkflowCommon;


namespace Workflow.Models.DAL
{
    public class WorkflowCreationDAO:ReturnData
    {
        DBConnection dbConn = new DBConnection();
        DBHelper dbHelper = new DBHelper();
        TerminalLogger terminal = new TerminalLogger();
        public bool SaveUpdate(WorkflowCreationBEO model)
        {
            bool isTrue = false;
          
           
            string CntDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string setTerminal = terminal.GetLanIPAddress();
                 
            try
            {
                string Qry = "";
                if (model.WorkflowId != null && model.WorkflowId != "")
                {
                    string QryIsExists = "Select * from WFM_StageMapDrawing Where  WorkflowId='" + model.WorkflowId + "'";
                    DataTable dt = dbHelper.GetDataTable(dbConn.DocConnStrReader(), QryIsExists);
                    if (dt.Rows.Count > 0) //NumberOfStage is no update
                    {
                        Qry = @"Update WFM_WorkflowCreation Set WorkflowName='" + model.WorkflowName + "',Remarks='" + model.Remarks + "',IsActive=" + (model.IsActive == false ? '0' : '1') + " Where WorkflowId='" + model.WorkflowId + "'";

                    }
                    else
                    {
                        Qry = @"Update WFM_WorkflowCreation Set WorkflowName='" + model.WorkflowName + "',NumberOfStage=" + model.NumberOfStage + ",Remarks='" + model.Remarks + "',IsActive=" + (model.IsActive == false ? '0' : '1') + " Where WorkflowId='" + model.WorkflowId + "'";
                    }
                        IUMode = "U";
                }
                else
                {
                    Qry = @"Insert Into WFM_WorkflowCreation(WorkflowName,NumberOfStage,Remarks,CategoryId,SetEmployeeId,SetDate,IsActive) 
                          Values('" + model.WorkflowName + "'," + model.NumberOfStage + ",'" + model.Remarks + "','" + model.CategoryId + "','" + model.SetEmployeeId + "','" + CntDate + "'," + (model.IsActive == false ? '0' : '1') + ")";
                    IUMode = "I";
                }
                if (dbHelper.CmdExecute(dbConn.DocConnStrReader(), Qry))
                {
                    isTrue = true;                 
                }
                return isTrue;
            }
            catch (Exception errorException)
            {
                throw errorException;
            }
        }

        public Tuple<string, DataTable, List<WorkflowCreationBEO>> GetWorkflowList(string CategoryId, int page, int itemsPerPage, string searchAttribute)
        {
            List<WorkflowCreationBEO> list = null;

            string Qry = @"DECLARE @page INT = " + page + ", @itemsPerPage INT = " + itemsPerPage + ";";
            Qry = Qry + @"Select ROW_NUMBER() OVER (ORDER BY A.WorkflowID) AS Col1,A.WorkflowId,A.WorkflowName,A.NumberOfStage,A.Remarks,A.CategoryId,A.SetEmployeeId,A.SetDate,A.IsActive, A.SetDate,
                        C.CategoryName,REPLACE(C.CategoryName+'/'+A.WorkflowName, ' ', '') As FileDirectory,COUNT(*) OVER () AS TotalCount
                        from WFM_WorkflowCreation As  A,Doc_Category As C
                        Where A.CategoryId=C.CategoryId ";

            //if (FromDate != "" && FromDate != null && FromDate != "" && FromDate != null)
            //{
            //    Qry = Qry + " AND A.SetDate Between '" + FromDate + "' AND '" + ToDate + "'";
            //}
            if (CategoryId != "" && CategoryId!=null)
            {
                Qry = Qry + " AND A.CategoryId='" + CategoryId + "'";
            }
            if (searchAttribute != "" && searchAttribute!=null)
            {
                Qry = Qry + " AND A.WorkflowName LIKE '%" + searchAttribute + "%'";
            }
            Qry = Qry + @" ORDER BY A.WorkflowId 
                        OFFSET ((@page - 1) * @itemsPerPage) ROWS FETCH NEXT @itemsPerPage ROWS ONLY;";
            DataTable dt = dbHelper.GetDataTable(dbConn.DocConnStrReader(), Qry);

            // DataTable dt = dbHelper.dtIncremented(dt2);
            var totalPages = dt.Rows.Count > 0 ? dt.Rows[0]["TotalCount"] : 0;
            list = (from DataRow row in dt.Rows
                    select new WorkflowCreationBEO
                    {
                        SL = row["Col1"].ToString(),
                        WorkflowId = row["WorkflowId"].ToString(),
                        WorkflowName = row["WorkflowName"].ToString(),
                        CategoryId = row["CategoryId"].ToString(),
                        CategoryName = row["CategoryName"].ToString(),
                        NumberOfStage = row["NumberOfStage"].ToString(),
                        Remarks = row["Remarks"].ToString(),
                        FileDirectory = row["FileDirectory"].ToString(),
                        IsActive = Convert.ToBoolean(row["IsActive"].ToString())
                      

                    }).ToList();
            return Tuple.Create(totalPages.ToString(), dt, list);
        }
    }
}