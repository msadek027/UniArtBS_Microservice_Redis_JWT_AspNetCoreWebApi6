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
    public class WorkflowStageMkCkDAO:ReturnData
    {
        DBConnection dbConn = new DBConnection();
        DBHelper dbHelper = new DBHelper();
        TerminalLogger terminal = new TerminalLogger();  
      
        public StageUserMappingBEO GetStageAndUserPermission(string WorkflowId, string StageId, string StageSL, string NumberOfStage)
        {
            //string setEmployeeId = HttpContext.Current.Session["EmployeeId"] as string;
            //string CntDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            //string setTerminal = terminal.GetLanIPAddress();

            StageUserMappingBEO objTemp =new StageUserMappingBEO();
            string Qry = @" Select A.StageId,A.WorkflowId,A.IsActive,B.StageSL,B.StageName,C.WorkflowName,C.NumberOfStage,C.CategoryId,REPLACE(D.CategoryName, ' ', '')  As CategoryName  
                        from WFM_StageSetup_UserPermission As A,WFM_StageSetup As B,WFM_WorkflowCreation As C,Doc_Category D
                        Where A.StageId=B.StageId AND A.WorkflowId=B.WorkflowId And A.WorkflowId=C.WorkflowId  AND C.CategoryId=D.CategoryId And A.StageId='" + StageId + "' AND  A.WorkflowId ='" + WorkflowId + "' ";
            DataTable dtTemp = dbHelper.GetDataTable(dbConn.DocConnStrReader(), Qry);
            DataTable dt = dbHelper.dtIncremented(dtTemp);
            if (dt.Rows.Count > 0)
            {             
                objTemp.StageId = dt.Rows[0]["StageId"].ToString();
                objTemp.WorkflowId = dt.Rows[0]["WorkflowId"].ToString();
                objTemp.StageSL = dt.Rows[0]["StageSL"].ToString();
                objTemp.IsFirstStageUser = dt.Rows[0]["StageSL"].ToString() == "1" ? true : false;
                objTemp.IsIntermediateStageUser = (dt.Rows[0]["StageSL"].ToString() != "1" && dt.Rows[0]["StageSL"].ToString() != NumberOfStage) ? true : false;
                objTemp.IsFinalStageUser = dt.Rows[0]["StageSL"].ToString() == NumberOfStage ? true : false;
                objTemp.StageName = dt.Rows[0]["StageName"].ToString();
                objTemp.WorkflowName = dt.Rows[0]["WorkflowName"].ToString();
                objTemp.CategoryName = dt.Rows[0]["CategoryName"].ToString();
            }

            return objTemp;
        }

      

        public DataTable GetWorkflowMkCkDocuments(string WorkflowId, string StageId, string StageSL, string NumberOfStage)
        {

            string Qry =@"Select A.ObjectId,A.IsBack,A.IsMakeCheck,A.StageId,A.WorkflowId,B.StageSL,C.NumberOfStage,COUNT(*) OVER () AS COUNT  
                        from WFM_Object_NextStage As A, WFM_StageSetup As B,WFM_WorkflowCreation As C
                        Where A.StageId=B.StageId And A.WorkflowId=C.WorkflowId 
                        And B.WorkflowId='" + WorkflowId + "' And A.StageId='" + StageId + "' And A.IsMakeCheck=0 Order By A.ObjectId DESC";

            DataTable dtTemp = dbHelper.GetDataTable(dbConn.DocConnStrReader(), Qry);
            DataTable dt = dbHelper.dtDecremented(dtTemp);
            
            if (dt.Rows.Count == 0)
            {
                dt.Columns.Add("ObjectId", typeof(string));
                dt.Columns.Add("IsBack", typeof(bool));
                dt.Columns.Add("IsMakeCheck", typeof(bool));
                dt.Columns.Add("StageId", typeof(string));
                dt.Columns.Add("WorkflowId", typeof(string));
                dt.Columns.Add("StageSL", typeof(int));
                dt.Columns.Add("NumberOfStage", typeof(int));
                dt.Columns.Add("COUNT", typeof(int));

                // Add an empty row
                // dt.Rows.Add(dt.NewRow());
            }
            return dt;
        }

        public WorkflowStageMkCkBEO GetALLDocsProp(string WorkflowId, string StageId, string StageSL, string NumberOfStage)
        {
            WorkflowStageMkCkBEO objTemp = new WorkflowStageMkCkBEO();

            string QryDoc = @" Select DocumentTagId,DocumentTagName,DocumentTagSL,IsRequired,StageId,WorkflowId  from WFM_StageSetup_DocumentTag         
                               Where  WorkflowId='" + WorkflowId + "' --And StageId ='" + StageId + "'";

           
            DataTable dtDoc = dbHelper.GetDataTable(dbConn.DocConnStrReader(), QryDoc);

            objTemp.Documents = (from DataRow row in dtDoc.Rows
                             select new StageDocumentTagBEO
                             {
                                 DocumentTagId = row["DocumentTagId"].ToString(),
                                 DocumentTagName = row["DocumentTagName"].ToString(),
                                 DocumentTagSL = (int)row["DocumentTagSL"],
                                 IsRequired = row["IsRequired"].ToString(),                 
                                 StageId = row["StageId"].ToString(),
                                 WorkflowId = row["WorkflowId"].ToString(),
                             

                             }).ToList();

            string QryAttribute = @" Select AttributeTagId,AttributeTagName,AttributeTagType, AttributeTagSL, IsRequired,StageId, WorkflowId  from WFM_StageSetup_DocumentAttributeTag
                                     Where WorkflowId='" + WorkflowId + "' --And StageId ='" + StageId + "' ";
         

            DataTable dtAttribute = dbHelper.GetDataTable(dbConn.DocConnStrReader(), QryAttribute);

            objTemp.TypeProperties = (from DataRow row in dtAttribute.Rows
                    select new DocumentAttributeTagBEO
                    {
                        AttributeTagId = row["AttributeTagId"].ToString(),
                        AttributeTagName = row["AttributeTagName"].ToString(),
                        AttributeTagType = row["AttributeTagType"].ToString(),
                        AttributeTagSL = row["AttributeTagSL"].ToString(),
                        IsRequired = Convert.ToBoolean(row["IsRequired"].ToString()),
              
                        StageId = row["StageId"].ToString(),
                        WorkflowId = row["WorkflowId"].ToString(),                    

                    }).ToList();

            string QryListAttribute = @" Select AttributeTagName,AttributeTagType, AttributeTagSL, IsRequired,StageId, WorkflowId  from WFM_StageSetup_DocumentAttributeTag
                   Where   WorkflowId = '100' And StageId = '" + StageId + "' ";
            DataTable dtListAttribute = dbHelper.GetDataTable(dbConn.DocConnStrReader(), QryListAttribute);

            objTemp.ListTypeProperties = (from DataRow row in dtListAttribute.Rows
                                  select new VM_ListTypeProperties
                                  {                                     
                                      TableRefID = row["StageId"].ToString(),
                                   //   AttributeTagName = row["WorkflowId"].ToString(),                             

                                  }).ToList();

         

          return objTemp;
        }

        public WorkflowStageTagValueMkCkBEO GetDocumentPropertyValues(string objectId, string stageId, string workflowId)
        {
            WorkflowStageTagValueMkCkBEO objTemp = new WorkflowStageTagValueMkCkBEO();

            objTemp.IsBacked=false;
            objTemp.BackReason="";

            string QryDocValue = @"Select B.ObjectId,A.DocumentTagId,A.DocumentTagName,A.DocumentTagSL,A.IsRequired,A.StageId,A.WorkflowId,B.DocumentTagValue,B.FileDirectory,C.StageName,S.FtpServerIP,S.FtpPort,S.FtpUserId,S.FtpPassword
                                   from WFM_StageSetup_DocumentTag As A,WFM_Object_DocumentTagValue AS B,WFM_StageSetup as C,Doc_Document_FtpServer As S
                                   Where A.DocumentTagId=B.DocumentTagId  AND A.StageId=C.StageId and  A.WorkflowId= C.WorkflowId And B.ObjectId+'_W'+ B.DocumentTagId=S.DocumentId     
                                  And  B.ObjectId='" + objectId + "' and A.WorkflowId='" + workflowId + "' ";


            DataTable dtDocValue = dbHelper.GetDataTable(dbConn.DocConnStrReader(), QryDocValue);

            objTemp.documentList = (from DataRow row in dtDocValue.Rows
                                 select new StageDocumentTagValueBEO
                                 {
                                     ObjectId = row["ObjectId"].ToString(),
                                     DocumentTagId = row["DocumentTagId"].ToString(),
                                     DocumentTagName = row["DocumentTagName"].ToString(),
                                     DocumentTagSL = (int)row["DocumentTagSL"],
                                     IsRequired = row["IsRequired"].ToString(),
                                    
                                     DocumentTagValue = row["DocumentTagValue"].ToString(),
                                     IsSelected = row["DocumentTagValue"].ToString()!=null&& row["DocumentTagValue"].ToString()!=""?true:false,
                                     IsBacked =false,
                                     BackReason="",
                                     StageId = row["StageId"].ToString(),
                                     StageName = row["StageName"].ToString(),
                                     WorkflowId = row["WorkflowId"].ToString(),
                                     FileDirectory = row["FileDirectory"].ToString(),

                                     FtpServerIP = row["FtpServerIP"].ToString(),
                                     FtpPort = row["FtpPort"].ToString(),
                                     FtpUserId = row["FtpUserId"].ToString(),
                                     FtpPassword = row["FtpPassword"].ToString(),

                                 }).ToList();


            string QryDoc = @" Select DocumentTagId,DocumentTagName,DocumentTagSL,IsRequired,StageId,WorkflowId  from WFM_StageSetup_DocumentTag         
            Where  WorkflowId='" + workflowId + "' --And StageId ='" + stageId + "'";

            DataTable dtDoc = dbHelper.GetDataTable(dbConn.DocConnStrReader(), QryDoc);

            objTemp.updateDocuments = (from DataRow row in dtDoc.Rows
                                 select new StageDocumentTagValueBEO
                                 {
                                     DocumentTagId = row["DocumentTagId"].ToString(),
                                     DocumentTagName = row["DocumentTagName"].ToString(),
                                     DocumentTagSL = (int)row["DocumentTagSL"],
                                     IsRequired = row["IsRequired"].ToString(),
                                     StageId = row["StageId"].ToString(),
                                     WorkflowId = row["WorkflowId"].ToString(),


                                 }).ToList();



            string QryAttribute = @" Select B.ObjectId,A.AttributeTagId,A.AttributeTagName,A.AttributeTagType, A.AttributeTagSL, A.IsRequired,A.StageId, A.WorkflowId,B.AttributeTagValue
                                    from WFM_StageSetup_DocumentAttributeTag As A,WFM_Object_DocumentAttributeTagValue As B
                                    Where  A.AttributeTagId= B.AttributeTagId  And B.ObjectId='" + objectId + "' and A.WorkflowId='" + workflowId + "'";


            DataTable dtAttribute = dbHelper.GetDataTable(dbConn.DocConnStrReader(), QryAttribute);

            objTemp.attributeList = (from DataRow row in dtAttribute.Rows
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

            string QryListAttribute = @" Select ObjectId,AttributeTagId,AttributeTagValue,StageId, WorkflowId  from WFM_Object_DocumentAttributeTagValue
                                   Where  ObjectId='" + objectId + "' AND StageId ='" + stageId + "' AND WorkflowId='" + workflowId + "'";
            DataTable dtListAttribute = dbHelper.GetDataTable(dbConn.DocConnStrReader(), QryListAttribute);

            objTemp.ListTypeProperties = (from DataRow row in dtListAttribute.Rows
                                          select new VM_ListTypeProperties
                                          {

                                              TableRefID = row["StageId"].ToString(),
                                              //   AttributeTagName = row["WorkflowId"].ToString(),



                                          }).ToList();

            //List<DataTable> listProps = new List<DataTable>();
            //if (dtDoc.Rows.Count > 0)
            //{
            //    listProps.Add(dtDoc);
            //}
            //objTemp.ListProperties = listProps;

            return objTemp;
        }

        public bool SetRevertFromMake(string ObjectId, string StageId, int StageSL, string WorkflowId, int NumberOfStage, List<StageDocumentTagValueBEO> documentList, string revertReason)
        {
            bool isTrue = false;
      
            string CntDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string setTerminal = terminal.GetLanIPAddress();
            if (documentList != null)
            {
                foreach (StageDocumentTagValueBEO model in documentList)
                {
                    isTrue = true;                
                    string ToStageId = model.StageId;
                    string QryDocHistory = @"INSERT INTO WFM_Object_StageChangeLogHistory(ObjectID, FromStage, ToStage, WorkflowId,IsBackReason,SetEmployeeId, SetDate) VALUES('" + ObjectId + "','" + StageId + "','" + ToStageId + "','" + WorkflowId + "','"+ revertReason + "', '" + model.SetEmployeeId + "','" + CntDate + "')";
                    dbHelper.CmdExecute(dbConn.DocConnStrReader(), QryDocHistory);

                    string QryNextStageOwn = @"update  WFM_Object_NextStage Set IsMakeCheck=1,IsBack=1 Where ObjectID='" + ObjectId + "' and WorkflowId='" + WorkflowId + "' and StageId='" + StageId + "'";
                    dbHelper.CmdExecute(dbConn.DocConnStrReader(), QryNextStageOwn);

                    string QryNextStage2 = @"INSERT INTO WFM_Object_NextStage(ObjectID,IsMakeCheck,IsBack, WorkflowId,StageId) VALUES('" + ObjectId + "',0,1,'" + WorkflowId + "','" + ToStageId + "')";
                    dbHelper.CmdExecute(dbConn.DocConnStrReader(), QryNextStage2);
                }
            }
         
            return isTrue;
        }

        public bool SetMakeDone(string ObjectId, string StageId,  int StageSL, string WorkflowId, int NumberOfStage, string SetEmployeeId, string CurrentURL, string SetTerminal, List<StageDocumentTagValueBEO> documentList, List<DocumentAttributeTagValueBEO> attributeList)
        {
            bool isTrue = false;
          
            string CntDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string setTerminal = terminal.GetLanIPAddress();
            var documentListOp= documentList!=null? documentList.Where(p => p.StageId == StageId.ToString()).ToList(): documentList;
            var attributeListOp = attributeList.Where(p => p.StageId == StageId.ToString()).ToList();
            if (documentListOp != null)
            {
                foreach (StageDocumentTagValueBEO model in documentListOp)
                {
                    isTrue = false;
                    string QryDoc = @"Update WFM_Object_DocumentTagValue Set DocumentTagValue='" + model.DocumentTagName + "' Where ObjectId='" + ObjectId + "' And StageId='" + model.StageId + "' And WorkflowId='" + model.WorkflowId + "'";
                    dbHelper.CmdExecute(dbConn.DocConnStrReader(), QryDoc);

                    string DocumentId = ObjectId + "_W" + model.DocumentTagId;
                    terminal.OperationalLogTaleCallFromDAO(DocumentId, "ObjectId_DocumentTagId", "WFM_Object_DocumentTagValue", "Insert", SetEmployeeId, CurrentURL,SetTerminal);
                }
            }
            if (attributeListOp != null)
            {
                foreach (DocumentAttributeTagValueBEO model in attributeListOp)
                {
                    isTrue = false;
                    string QryAttribute = @"Update WFM_Object_DocumentAttributeTagValue Set AttributeTagValue='" + model.AttributeTagValue + "' Where ObjectId='" + ObjectId + "' And StageId='" + model.StageId + "' And WorkflowId='" + model.WorkflowId + "'";
                    dbHelper.CmdExecute(dbConn.DocConnStrReader(), QryAttribute);
                }
            }
            int attributeListCount = attributeList != null ? attributeList.Count : 0; 
            int documentListCount = documentList != null ? documentList.Count : 0;
      
            if (attributeListCount >= documentListCount)
            {
                int index = attributeList.FindIndex(a => a.StageId == StageId);
                int nextIndex = index == attributeList.Count - 1 ? index : index + 1;
                string ToStageId = attributeList[nextIndex].StageId;
                string QryDocHistory = @"INSERT INTO WFM_Object_StageChangeLogHistory(ObjectID, FromStage, ToStage, WorkflowId,IsBackReason,SetEmployeeId, SetDate) VALUES('" + ObjectId + "','" + StageId + "','" + ToStageId + "','" + WorkflowId + "', NULL, '" + SetEmployeeId + "','" + CntDate + "')";
                dbHelper.CmdExecute(dbConn.DocConnStrReader(), QryDocHistory);                
                
                 string QryNextStageOwn = @"update  WFM_Object_NextStage Set IsMakeCheck=1 Where ObjectID='" + ObjectId + "' and WorkflowId='" + WorkflowId + "' and StageId='" + StageId + "'";
                dbHelper.CmdExecute(dbConn.DocConnStrReader(), QryNextStageOwn);

                if (attributeList.Count != StageSL)
                {
                    string QryNextStage2 = @"INSERT INTO WFM_Object_NextStage(ObjectID,IsMakeCheck,IsBack, WorkflowId,StageId) VALUES('" + ObjectId + "',0,0,'" + WorkflowId + "','" + ToStageId + "')";
                    dbHelper.CmdExecute(dbConn.DocConnStrReader(), QryNextStage2);
                }
                string vPassedKey = attributeList.Count == StageSL ? "1" : "0";
                string QryObj = @"Update WFM_Object Set PassedKey=" + vPassedKey + " Where  ObjectId='" + ObjectId + "'";
                dbHelper.CmdExecute(dbConn.DocConnStrReader(), QryObj);
                isTrue = true;
            }
            else
            {
                int index = documentList.FindIndex(a => a.StageId == StageId);
                int nextIndex = index == documentList.Count - 1 ? index : index + 1;
                string ToStageId = documentList[nextIndex].StageId;
                string QryDocHistory = @"INSERT INTO WFM_Object_StageChangeLogHistory(ObjectID, FromStage, ToStage, WorkflowId,IsBackReason,SetEmployeeId, SetDate) VALUES('" + ObjectId + "','" + StageId + "','" + ToStageId + "', NULL, '" + SetEmployeeId + "','" + CntDate + "')";
                dbHelper.CmdExecute(dbConn.DocConnStrReader(), QryDocHistory);

                string QryNextStageOwn = @"update  WFM_Object_NextStage Set IsMakeCheck=1 Where ObjectID='" + ObjectId + "' and WorkflowId='" + WorkflowId + "' and StageId='" + StageId + "'";
                dbHelper.CmdExecute(dbConn.DocConnStrReader(), QryNextStageOwn);

                if (documentList.Count != StageSL)
                {
                    string QryNextStage2 = @"INSERT INTO WFM_Object_NextStage(ObjectID,IsMakeCheck,IsBack, WorkflowId,StageId) VALUES('" + ObjectId + "',0,0,'" + WorkflowId + "','" + ToStageId + "')";
                    dbHelper.CmdExecute(dbConn.DocConnStrReader(), QryNextStage2);
                }
             
                string vPassedKey = documentList.Count == StageSL ? "1" : "0";
                string QryObj = @"Update WFM_Object Set PassedKey=" + vPassedKey + " Where  ObjectId='" + ObjectId + "'";
                dbHelper.CmdExecute(dbConn.DocConnStrReader(), QryObj);
                isTrue = true;
            }


            return isTrue;
        }

        public bool AddDocumentInfo(ObjectBEO obj,List<StageDocumentTagValueBEO> documentList, List<DocumentAttributeTagValueBEO> attributeList, string sqlTxt)
        {
            bool isTrue = false;        
            string CntDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");       

            string QryObj = @"Insert Into WFM_Object(ObjectId,WorkflowId,PassedKey,IsActive,SetEmployeeId,SetDate) 
                          Values('" + obj.ObjectId + "','" + obj.WorkflowId + "',0,1,'" + obj.SetEmployeeId + "','" + CntDate + "')";
            dbHelper.CmdExecute(dbConn.DocConnStrReader(), QryObj);


            if (documentList != null)
            {
                foreach (StageDocumentTagValueBEO model in documentList)
                {
                    isTrue = false;
                    model.DocumentTagValue = model.IsSelected == true ? model.DocumentTagName : model.DocumentTagValue;
                    model.FileDirectory = model.FileDirectory.Replace(" ", "");
                    string QryDoc = @"Insert Into WFM_Object_DocumentTagValue(ObjectId,DocumentTagId,DocumentTagValue,StageId,WorkflowId,FileDirectory) 
                          Values('" + obj.ObjectId + "','" + model.DocumentTagId + "','" + model.DocumentTagValue + "','" + model.StageId + "','" + model.WorkflowId + "','" + model.FileDirectory + "')";
                    dbHelper.CmdExecute(dbConn.DocConnStrReader(), QryDoc);
                    if (model.IsSelected == true)
                    {
                        string DocumentId = obj.ObjectId + "_W" + model.DocumentTagId;
                        terminal.OperationalLogTaleCallFromDAO(DocumentId, "ObjectId_DocumentTagId", "WFM_Object_DocumentTagValue", "Insert", obj.SetEmployeeId, obj.CurrentURL, obj.SetTermial);
                    }
                }
            }
            if (attributeList != null)
            {
                foreach (DocumentAttributeTagValueBEO model in attributeList)
                {
                    isTrue = false;
                    string QryAttribute = @"Insert Into WFM_Object_DocumentAttributeTagValue(ObjectId,AttributeTagId,AttributeTagValue,StageId,WorkflowId) 
                          Values('" + obj.ObjectId + "','" + model.AttributeTagId + "','" + model.AttributeTagValue + "','" + model.StageId + "','" + model.WorkflowId + "')";
                    dbHelper.CmdExecute(dbConn.DocConnStrReader(), QryAttribute);

                }
            }
         
            int countDocumentList = (documentList?.Count) ?? 0;
            if (attributeList.Count>= countDocumentList)
            {
                int index = attributeList.FindIndex(a => a.StageId == obj.StageId);
                int nextIndex = index == attributeList.Count - 1 ? index : index + 1;
                string ToStageId = attributeList[nextIndex].StageId;
                string QryDocHistory = @"INSERT INTO WFM_Object_StageChangeLogHistory(ObjectID, FromStage, ToStage, WorkflowId,IsBackReason,SetEmployeeId, SetDate) VALUES('" + obj.ObjectId + "','" + obj.StageId + "','" + ToStageId + "','" + obj.WorkflowId + "', NULL, '" + obj.SetEmployeeId + "','" + CntDate + "')";
                dbHelper.CmdExecute(dbConn.DocConnStrReader(), QryDocHistory);

                string QryNextStage1 = @"INSERT INTO WFM_Object_NextStage(ObjectID,IsMakeCheck,IsBack, WorkflowId,StageId) VALUES('" + obj.ObjectId + "',1,0,'" + obj.WorkflowId + "','" + obj.StageId + "')";
                dbHelper.CmdExecute(dbConn.DocConnStrReader(), QryNextStage1);
                string QryNextStage2 = @"INSERT INTO WFM_Object_NextStage(ObjectID,IsMakeCheck,IsBack, WorkflowId,StageId) VALUES('" + obj.ObjectId + "',0,0,'" + obj.WorkflowId + "','" + ToStageId + "')";
                dbHelper.CmdExecute(dbConn.DocConnStrReader(), QryNextStage2);      
            
            }
            else
            {
                int index = documentList.FindIndex(a => a.StageId == obj.StageId);
                int nextIndex = index == documentList.Count - 1 ? index : index + 1;
                string ToStageId = documentList[nextIndex].StageId;
                string QryDocHistory = @"INSERT INTO WFM_Object_StageChangeLogHistory(ObjectID, FromStage, ToStage, WorkflowId,IsBackReason,SetEmployeeId, SetDate) VALUES('" + obj.ObjectId + "','" + obj.StageId + "','" + ToStageId + "', NULL, '" + obj.SetEmployeeId + "','" + CntDate + "')";
                dbHelper.CmdExecute(dbConn.DocConnStrReader(), QryDocHistory);

                string QryNextStage1 = @"INSERT INTO WFM_Object_NextStage(ObjectID,IsMakeCheck,IsBack, WorkflowId,StageId) VALUES('" + obj.ObjectId + "',1,0,'" + obj.WorkflowId + "','" + obj.StageId + "')";
                dbHelper.CmdExecute(dbConn.DocConnStrReader(), QryNextStage1);
                string QryNextStage2 = @"INSERT INTO WFM_Object_NextStage(ObjectID,IsMakeCheck,IsBack, WorkflowId,StageId) VALUES('" + obj.ObjectId + "',0,0,'" + obj.WorkflowId + "','" + ToStageId + "')";
                dbHelper.CmdExecute(dbConn.DocConnStrReader(), QryNextStage2);
                isTrue = true;
            }
 

        
            return isTrue;
        }

     
    }
}