using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using Workflow.Models.BEL;
using Workflow.WorkflowCommon;


namespace Workflow.Models.DAL
{
    public class RptWorkflowDAO
    {
        DBConnection dbConn = new DBConnection();
        DBHelper dbHelper = new DBHelper();
    
        public Tuple<string, DataTable, List<RptWorkflowBEO>> GetObjectList(string WorkflowId, int page, int itemsPerPage, string searchAttribute)
        {
            List<RptWorkflowBEO> list = null;

            string Qry = @"WFM_Rpt_GetWorkflow @WorkflowId='"+ WorkflowId + "',@page="+page+", @itemsPerPage= "+itemsPerPage+", @search= '"+searchAttribute+"'";

            //if (FromDate != "" && FromDate != null && FromDate != "" && FromDate != null)
            //{
            //    Qry = Qry + " AND A.SetDate Between '" + FromDate + "' AND '" + ToDate + "'";
            //}
       
            DataTable dtTemp = dbHelper.GetDataTable(dbConn.DocConnStrReader(), Qry);

           // DataTable dt = dbHelper.dtIncremented(dtTemp);
            DataTable dt = dbHelper.dtDecremented(dtTemp);
            var totalPages = dt.Rows.Count > 0 ? dt.Rows[0]["TotalCount"] : 0;
            list = (from DataRow row in dt.Rows
                    select new RptWorkflowBEO
                    {
                        SL = row["S/L"].ToString(),
                        ObjectId = row["ObjectId"].ToString(),
                        //WorkflowName = row["WorkflowName"].ToString(),
                        //CategoryId = row["CategoryId"].ToString(),
                        //NumberOfStage = row["NumberOfStage"].ToString(),
                        //Remarks = row["Remarks"].ToString(),
                        //PhysicalFileDirectory = row["PhysicalFileDirectory"].ToString(),
                        //IsActive = Convert.ToBoolean(row["IsActive"].ToString())


                    }).ToList();
            return Tuple.Create(totalPages.ToString(), dt, list);
        }

        public List<RptWorkflowBEO> GetDocumentForSpecificObject(string ObjectId, string UserID)
        {
            
            string Qry = @"Select A.ObjectId+'_W'+A.DocumentTagId As FileId,A.DocumentTagValue,A.FileDirectory,S.FtpServerIP,S.FtpPort,S.FtpUserId,S.FtpPassword 
                          from WFM_Object_DocumentTagValue As A,Doc_Document_FtpServer As S
                          Where A.ObjectId+'_W'+A.DocumentTagId=S.DocumentId AND  A.ObjectId = '" + ObjectId + "'";

            DataTable dt2 = dbHelper.GetDataTable(dbConn.DocConnStrReader(), Qry);
            DataTable dt = dbHelper.dtIncremented(dt2);
            List<RptWorkflowBEO> list = null;
            list = (from DataRow row in dt.Rows
                    select new RptWorkflowBEO
                    {
                        SL = row["Col1"].ToString(),
                        DocumentId = row["FileId"].ToString(),
                        DocumentTagValue = row["DocumentTagValue"].ToString(),
                        FileDirectory = row["FileDirectory"].ToString(),

                        FtpServerIP = row["FtpServerIP"].ToString(),
                        FtpPort = row["FtpPort"].ToString(),
                        FtpUserId = row["FtpUserId"].ToString(),
                        FtpPassword = row["FtpPassword"].ToString(),

                    }).ToList();



            return list;
        }

        public WorkflowStageDocumentAttributeReportBEO GetWorkflowStageDocumentAttributeValues(string objectId)
        {

            WorkflowStageDocumentAttributeReportBEO obj = new WorkflowStageDocumentAttributeReportBEO();


            string QryWf = @"Select A.WorkflowId,A.WorkflowName,A.NumberOfStage,B.ObjectId from WFM_WorkflowCreation AS A INNER JOIN   
                            (Select Distinct ObjectId, WorkflowId from WFM_Object_DocumentTagValue) As B
                            ON A.WorkflowId = A.WorkflowId And B.ObjectId ='" + objectId + "'";
            DataTable dtWf = dbHelper.GetDataTable(dbConn.DocConnStrReader(), QryWf);

            WorkflowCreationBEO workflow = new WorkflowCreationBEO();
            if (dtWf.Rows.Count > 0)
            {
                workflow.WorkflowId = dtWf.Rows[0]["WorkflowId"].ToString();
                workflow.WorkflowName = dtWf.Rows[0]["WorkflowName"].ToString();
                workflow.NumberOfStage = dtWf.Rows[0]["NumberOfStage"].ToString();
                obj.workflow = workflow;
            }
            else
            {
                string QryWithoutFile = "Select Distinct A.WorkflowId,B.WorkflowName,B.NumberOfStage from WFM_Object A,WFM_WorkflowCreation B Where A.WorkflowId = B.WorkflowId AND A.ObjectId='"+ objectId + "'";

                dtWf = dbHelper.GetDataTable(dbConn.DocConnStrReader(), QryWithoutFile);
                workflow.WorkflowId = dtWf.Rows[0]["WorkflowId"].ToString();
                workflow.WorkflowName = dtWf.Rows[0]["WorkflowName"].ToString();
                workflow.NumberOfStage = dtWf.Rows[0]["NumberOfStage"].ToString();
                obj.workflow = workflow;
            }
            string QryDocValue = @"Select B.ObjectId,A.DocumentTagId,A.DocumentTagName,A.DocumentTagSL,A.IsRequired,A.StageId,A.WorkflowId,B.DocumentTagValue,B.FileDirectory,C.StageName
                                   from WFM_StageSetup_DocumentTag As A,WFM_Object_DocumentTagValue AS B,WFM_StageSetup as C
                                   Where A.DocumentTagId=B.DocumentTagId  AND A.StageId=C.StageId and  A.WorkflowId= C.WorkflowId      
                                  And  B.ObjectId='" + objectId + "'  ";


            DataTable dtDocValue = dbHelper.GetDataTable(dbConn.DocConnStrReader(), QryDocValue);
            obj.documentList = (from DataRow row in dtDocValue.Rows
                                select new StageDocumentTagValueBEO
                                {
                                    ObjectId = row["ObjectId"].ToString(),
                                    DocumentTagId = row["DocumentTagId"].ToString(),
                                    DocumentTagName = row["DocumentTagName"].ToString(),
                                    DocumentTagSL = (int)row["DocumentTagSL"],
                                    IsRequired = row["IsRequired"].ToString(),
                                    DocumentTagValue = row["DocumentTagValue"].ToString(),
                                    IsSelected = row["DocumentTagValue"].ToString() != null && row["DocumentTagValue"].ToString() != "" ? true : false,
                                    IsBacked = false,
                                    BackReason = "",
                                    StageId = row["StageId"].ToString(),
                                    StageName = row["StageName"].ToString(),
                                    WorkflowId = row["WorkflowId"].ToString(),
                                    FileDirectory = row["FileDirectory"].ToString(),
                                }).ToList();




            string QryAttribute = @" Select B.ObjectId,A.AttributeTagId,A.AttributeTagName,A.AttributeTagType, A.AttributeTagSL, A.IsRequired,A.StageId, A.WorkflowId,B.AttributeTagValue
                                    from WFM_StageSetup_DocumentAttributeTag As A,WFM_Object_DocumentAttributeTagValue As B
                                    Where  A.AttributeTagId= B.AttributeTagId  And B.ObjectId='" + objectId + "' ";
            DataTable dtAttribute = dbHelper.GetDataTable(dbConn.DocConnStrReader(), QryAttribute);
            obj.attributeList = (from DataRow row in dtAttribute.Rows
                                 select new DocumentAttributeTagValueBEO
                                 {
                                     ObjectId = row["ObjectId"].ToString(),
                                     AttributeTagId = row["AttributeTagId"].ToString(),
                                     AttributeTagName = row["AttributeTagName"].ToString(),
                                     AttributeTagValue = row["AttributeTagValue"].ToString(),
                                     AttributeTagType = row["AttributeTagType"].ToString(),
                                     AttributeTagSL = row["AttributeTagSL"].ToString(),
                                     IsRequired = Convert.ToBoolean(row["IsRequired"].ToString()),
                                     StageId = row["StageId"].ToString(),
                                     WorkflowId = row["WorkflowId"].ToString(),
                                 }).ToList();

            string QryLogHistory = @" Select ObjectId,FromStage,ToStage,IsBackReason,SetEmployeeId,SetDate from WFM_Object_StageChangeLogHistory Where ObjectId='" + objectId + "' ";
            DataTable dtLogHistory = dbHelper.GetDataTable(dbConn.DocConnStrReader(), QryLogHistory);
            obj.stageChangeLogHistoryList = (from DataRow row in dtLogHistory.Rows
                                 select new StageChangeLogHistoryBEO
                                 {
                                     SetEmployeeName = row["SetEmployeeId"].ToString(),
                                     SetDate = row["SetDate"].ToString(),
                                     FromStage = row["FromStage"].ToString(),
                                     ToStage = row["ToStage"].ToString(),
                               
                                 }).ToList();
            return obj;
        }
        public WorkflowStageDocumentAttributeReportBEO GetDocumentMergeFiles(string inClause)
        {
            WorkflowStageDocumentAttributeReportBEO obj = new WorkflowStageDocumentAttributeReportBEO();          

            string Qry = @"Select * from ( 
                            Select A.DocumentId,A.DocumentName,A.FileExtension,'Documents' As DataCategory,A.FileDirectory,A.SetDate,
                            S.FtpServerIP,S.FtpPort,S.FtpUserId,S.FtpPassword
                            from Doc_Documents As  A,Doc_Document_FtpServer As S
                            Where  A.DocumentId=S.DocumentId  
                            UNION ALL 
                            Select A.ObjectId+'_W'+A.DocumentTagId As DocumentId,A.DocumentTagValue As DocumentName,'pdf' As FileExtension,'Workflow' As DataCategory,A.FileDirectory,S.SetDate,S.FtpServerIP,S.FtpPort,S.FtpUserId,S.FtpPassword 
                            from WFM_Object_DocumentTagValue As A,Doc_Document_FtpServer As S
                            Where A.ObjectId+'_W'+A.DocumentTagId=S.DocumentId
                            ) T Where T.DocumentId IN(" + inClause + ")";
            

            DataTable dtDocValue = dbHelper.GetDataTable(dbConn.DocConnStrReader(), Qry);
            obj.documentList = (from DataRow row in dtDocValue.Rows
                                select new StageDocumentTagValueBEO
                                {
                                    ObjectId = row["DocumentId"].ToString(),
                                    DocumentTagId = row["DocumentId"].ToString(),
                                    DocumentTagName = row["DocumentName"].ToString(),                                   
                                    FileDirectory = row["FileDirectory"].ToString(),
                                }).ToList();
            return obj;
        }
        public bool FileDocIsExists(string docQry)
        {
            DataTable dt = dbHelper.GetDataTable(dbConn.DocConnStrReader(), docQry);
            if(dt.Rows.Count>0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}