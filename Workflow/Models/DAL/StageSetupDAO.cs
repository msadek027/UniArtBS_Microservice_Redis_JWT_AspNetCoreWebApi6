using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Workflow.Models.BEL;
using Workflow.WorkflowCommon;


namespace Workflow.Models.DAL
{
    public class StageSetupDAO:ReturnData
    {
        DBConnection dbConn = new DBConnection();
        DBHelper dbHelper = new DBHelper();
        TerminalLogger terminal = new TerminalLogger();
        public List<StageSetupBEO> GetGenerateStageSetupList(string workflowId)
        {
            List<StageSetupBEO> list = null;
            string QryPostStage = @"Select StageId,StageName,StageSL,DefaultHaveMk,DefaultHaveCk,DefaultNotifyMk,DefaultNotifyCk,WorkflowId,SetEmployeeId,SetDate,IsActive
                                   from WFM_StageSetup Where WorkflowId='" + workflowId + "' AND IsActive=1";
            string QryPreStage = @"Select WorkflowId,CategoryId,WorkflowName,NumberOfStage 
                                   from WFM_WorkflowCreation  Where WorkflowId='" + workflowId + "' AND IsActive=1 ";


            DataTable dtPreStage = dbHelper.GetDataTable(dbConn.DocConnStrReader(), QryPreStage);
            DataTable dtPostStage = dbHelper.GetDataTable(dbConn.DocConnStrReader(), QryPostStage);
            int val = Convert.ToInt16(dtPreStage.Rows[0]["NumberOfStage"]);

            DataTable dataTable = new DataTable("Stage");

            // Define columns
            dataTable.Columns.Add("WorkflowId", typeof(string));
            //dataTable.Columns.Add("WorkflowName", typeof(string));
       
            dataTable.Columns.Add("StageSL", typeof(string));
            dataTable.Columns.Add("StageId", typeof(string));
            dataTable.Columns.Add("StageName", typeof(string));
            dataTable.Columns.Add("HaveMk", typeof(Boolean));
            dataTable.Columns.Add("HaveCk", typeof(Boolean));
            dataTable.Columns.Add("IsActive", typeof(Boolean));


            if (dtPostStage.Rows.Count > 0)
            {
                IUMode = "PostStage";
                for (int i = 0; i < dtPostStage.Rows.Count; i++)
                {
                  
                    DataRow row = dataTable.NewRow();
                    row["WorkflowId"] = dtPostStage.Rows[i]["WorkflowId"];
                   // row["WorkflowName"] = "";//dtPostStage.Rows[i]["WorkflowName"];
           
                    row["StageSL"] = dtPostStage.Rows[i]["StageSL"];
                    row["StageId"] = dtPostStage.Rows[i]["StageId"];
                    row["StageName"] = dtPostStage.Rows[i]["StageName"];

                    row["HaveMk"] = Convert.ToBoolean(dtPostStage.Rows[i]["DefaultHaveMk"].ToString());
                    row["HaveCk"] = Convert.ToBoolean(dtPostStage.Rows[i]["DefaultHaveCk"].ToString());
                    row["IsActive"] = Convert.ToBoolean(dtPostStage.Rows[i]["IsActive"].ToString());

                    dataTable.Rows.Add(row);
                }
            }
            else
            {
                IUMode = "PreStage";
                for (int i = 0; i < val; i++)
                {
                    DataRow row = dataTable.NewRow();
                    row["WorkflowId"] = dtPreStage.Rows[0]["WorkflowId"];
                   // row["WorkflowName"] = dtPreStage.Rows[0]["WorkflowName"];
              
                    row["StageSL"] = i + 1;
                    row["StageId"] = "";
                    row["StageName"] = "";
                    if (i == 0)
                    {
                        row["HaveMk"] = true;
                        row["HaveCk"] = false;
                    }
                    else
                    {
                        row["HaveMk"] = false;
                        row["HaveCk"] = false;
                    }
                    row["IsActive"] = true;
                    dataTable.Rows.Add(row);
                }
            }
           
            list = (from DataRow row in dataTable.Rows
                    select new StageSetupBEO
                    {
                        WorkflowId = row["WorkflowId"].ToString(),
                       // WorkflowName = row["WorkflowName"].ToString(),
                   
                        StageSL = row["StageSL"].ToString(),
                        StageId = row["StageId"].ToString(),
                        StageName = row["StageName"].ToString(),                   
                        HaveMk =Convert.ToBoolean(row["HaveMk"].ToString()),
                        HaveCk = Convert.ToBoolean(row["HaveCk"].ToString()),
                        IsActive = Convert.ToBoolean(row["IsActive"].ToString()),

                    }).ToList();
            return  list;
        }

       

        public bool SaveUpdateDocument(StageDocumentTagBEO model, List<DocumentTagPermission> documentPermissionModal,string OpMode)
        {
         
            bool isTrue = false;
          
       
            string CntDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string setTerminal = terminal.GetLanIPAddress();
            model.IsActive = true;
            if (OpMode == "Save")
            {
                string Qry = @"Insert Into WFM_StageSetup_DocumentTag(DocumentTagId,DocumentTagName, DocumentTagSL, IsRequired,StageId, WorkflowId,SetEmployeeId,SetDate,IsActive) 
                          Values('" + model.DocumentTagId + "','" + model.DocumentTagName + "'," + model.DocumentTagSL + ",'" + model.IsRequired + "','" + model.StageId + "','" + model.WorkflowId + "','" + model.SetEmployeeId + "','" + CntDate + "'," + (model.IsActive == false ? '0' : '1') + ")";

                if (dbHelper.CmdExecute(dbConn.DocConnStrReader(), Qry))
                {
                    foreach (DocumentTagPermission detail in documentPermissionModal)
                    {
                        isTrue = false;
                        string QryDtl = @"Insert Into WFM_StageSetup_DocumentTagForStage(DocumentTagId,StageId, ForStageId,WorkflowId) 
                          Values('" + model.DocumentTagId + "','" + detail.StageId + "','" + model.StageId + "','" + model.WorkflowId + "')";
                        if (dbHelper.CmdExecute(dbConn.DocConnStrReader(), QryDtl))
                        {
                            IUMode = "I";
                            isTrue = true;
                        }
                    }
                }
            }
            else
            {
                string Qry = @"Update WFM_StageSetup_DocumentTag Set DocumentTagName='" + model.DocumentTagName + "',IsRequired='" + model.IsRequired + "' " +
                    " Where DocumentTagId='" + model.DocumentTagId + "' And  DocumentTagSL=" + model.DocumentTagSL + " And StageId='" + model.StageId + "' And WorkflowId='" + model.WorkflowId + "'"; 

                if (dbHelper.CmdExecute(dbConn.DocConnStrReader(), Qry))
                {
                    IUMode = "U";
                    isTrue = true;
                }

             }

            return isTrue;
        }

        public bool SaveUpdateDocumentAttribute(DocumentAttributeTagBEO model)
        {
            // WFM_StageSetup_DocumentAttribute
            bool isTrue = false;
    
            string CntDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string setTerminal = terminal.GetLanIPAddress();
            model.IsActive = true;

            DataTable tableTypeDataTable = new DataTable();
            if (model.AttributeTagType == "List")
            {
                tableTypeDataTable = new DataTable();
                tableTypeDataTable.Columns.Add("FieldTitle");
                tableTypeDataTable.Columns.Add("DataType");
                tableTypeDataTable.Columns.Add("MaxSize");
                tableTypeDataTable.Columns.Add("Master");

                foreach (var item in model.TableProperties)
                {
                    DataRow objDataRow = tableTypeDataTable.NewRow();

                    objDataRow[0] = item.FieldTitle;
                    objDataRow[1] = item.DataType;
                    objDataRow[2] = item.MaxSize;
                    objDataRow[3] = Convert.ToInt32(item.Master);

                    tableTypeDataTable.Rows.Add(objDataRow);
                }
            }

            if (model.OpMode == "Save")
            {
                string Qry = @"Insert Into WFM_StageSetup_DocumentAttributeTag(AttributeTagId,AttributeTagName,AttributeTagType, AttributeTagSL, IsRequired,StageId, WorkflowId, SetEmployeeId,SetDate,IsActive) 
                          Values('" + model.AttributeTagId + "','" + model.AttributeTagName + "','" + model.AttributeTagType + "'," + model.AttributeTagSL + ",'" + model.IsRequired + "','" + model.StageId + "','" + model.WorkflowId + "','" + model.SetEmployeeId + "','" + CntDate + "'," + (model.IsActive == false ? '0' : '1') + ")";
                if (dbHelper.CmdExecute(dbConn.DocConnStrReader(), Qry))
                {
                    IUMode = "I";
                    isTrue = true;
                }
            }
            else
            {
               
                string Qry = @"Update WFM_StageSetup_DocumentAttributeTag Set AttributeTagName='" + model.AttributeTagName + "',IsRequired='" + model.IsRequired + "',AttributeTagType='" + model.AttributeTagType + "'  " +
                  " Where AttributeTagId='" + model.AttributeTagId + "' And  AttributeTagSL=" + model.AttributeTagSL + " And StageId='" + model.StageId + "' And WorkflowId='" + model.WorkflowId + "'";

                if (dbHelper.CmdExecute(dbConn.DocConnStrReader(), Qry))
                {
                    IUMode = "U";
                    isTrue = true;
                }
            }
                return isTrue;
        }

        internal object GetChildStages(string stageMapId,string workflowId)
        {
          
            List<StageSetupStageMapBEO> list = null;
            string QryPostStage = @" Select A.StageMapId,A.StageId,B.StageName  from WFM_StageMap As A , WFM_StageSetup AS B
               Where A.StageId=B.StageId AND B.WorkflowId='"+ workflowId + "' And  A.StageMapId >" + stageMapId + " ";
            DataTable dtPostStage = dbHelper.GetDataTable(dbConn.DocConnStrReader(), QryPostStage);


            list = (from DataRow row in dtPostStage.Rows
                    select new StageSetupStageMapBEO
                    {
                        StageMapId = row["StageMapId"].ToString(),                       
                        StageId = row["StageId"].ToString(),
                        StageName = row["StageName"].ToString(),
                        IsChecked=true,

                    }).ToList();
            return list;
        }
        internal object GetGetChildStages_DocumentWisePermission(string stageMapId, string forStageId, string workflowId)
        {
            List<DocumentTagPermission> list = null;
            string QryPostStage = @" Select A.StageId,A.ForStageId,A.WorkflowId,B.StageName  from WFM_StageSetup_DocumentTagForStage As A , WFM_StageSetup AS B
               Where A.StageId=B.StageId AND B.WorkflowId='" + workflowId + "' And  A.ForStageId ='" + forStageId + "' ";
            DataTable dtPostStage = dbHelper.GetDataTable(dbConn.DocConnStrReader(), QryPostStage);


            list = (from DataRow row in dtPostStage.Rows
                    select new DocumentTagPermission
                    {
                       // StageMapId = row["StageMapId"].ToString(),
                        StageId = row["StageId"].ToString(),
                        StageName = row["StageName"].ToString(),
                        IsChecked = true,

                        WorkflowId = row["WorkflowId"].ToString(),
                      
                       

                    }).ToList();
            return list;
        }
        internal List<DocumentAttributeTagBEO> GetAllDocumentAttribute(string workflowId, string stageMapId, string stageId)
        {
            List<DocumentAttributeTagBEO> list = null;

          string QryPostStage = @" Select AttributeTagId,AttributeTagName,AttributeTagType, AttributeTagSL, IsRequired,StageId, WorkflowId  from WFM_StageSetup_DocumentAttributeTag
                   Where WorkflowId = '" + workflowId + "' And StageId = '" + stageId + "' ";
            DataTable dtPostStage = dbHelper.GetDataTable(dbConn.DocConnStrReader(), QryPostStage);


            list = (from DataRow row in dtPostStage.Rows
                    select new DocumentAttributeTagBEO
                    {
                        AttributeTagId = row["AttributeTagId"].ToString(),
                        AttributeTagName = row["AttributeTagName"].ToString(),
                        AttributeTagType = row["AttributeTagType"].ToString(),
                        AttributeTagSL = row["AttributeTagSL"].ToString(),
                        IsRequired = Convert.ToBoolean(row["IsRequired"].ToString()),
                       // StageMapId = row["StageMapId"].ToString(),
                        StageId = row["StageId"].ToString(),
                        WorkflowId = row["WorkflowId"].ToString(),
                     


                    }).ToList();
            return list;
        }

        internal List<StageDocumentTagBEO> GetAllDocuments(string workflowId, string stageMapId, string stageId)
        {
            List<StageDocumentTagBEO> list = null;
            string QryPostStage = @" Select DocumentTagId,DocumentTagName,DocumentTagSL,IsRequired,StageId,WorkflowId  from WFM_StageSetup_DocumentTag
               Where WorkflowId='" + workflowId + "' And  StageId ='" + stageId + "' ";
            DataTable dtPostStage = dbHelper.GetDataTable(dbConn.DocConnStrReader(), QryPostStage);


            list = (from DataRow row in dtPostStage.Rows
                    select new StageDocumentTagBEO
                    {
                        DocumentTagId = row["DocumentTagId"].ToString(),
                        DocumentTagName = row["DocumentTagName"].ToString(),
                        DocumentTagSL = (int)row["DocumentTagSL"],
                        IsRequired =row["IsRequired"].ToString(),                     
                        StageId = row["StageId"].ToString(),
                        WorkflowId = row["WorkflowId"].ToString(),                   

                    }).ToList();
            return list;
        }
        internal bool SaveUpdateStagePathDrawing(List<StagePathDrawingBEO> StagePathDrawingList, List<StageSetupBEO> StagePositionList)
        {
            bool isTrue = false;
           
            string CntDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string setTerminal = terminal.GetLanIPAddress();
            //Add For Update
            string QryDel = "Delete from WFM_StageMapDrawing Where WorkflowId='"+ StagePathDrawingList[0].WorkflowId + "'";
            dbHelper.CmdExecute(dbConn.DocConnStrReader(), QryDel);

            if (StagePositionList != null)
            {                
                foreach (StageSetupBEO model in StagePositionList)
                {                
                    isTrue = false;
                    string Qry = @"Update WFM_StageSetup Set PositionX=" + model.PositionX + ",PositionY=" + model.PositionY + " Where StageId='" + model.StageId + "'";
                     
                    if (dbHelper.CmdExecute(dbConn.DocConnStrReader(), Qry))
                    {
                        IUMode = "I";
                        isTrue = true;
                    }
                }
            }

            try
            {
                foreach (StagePathDrawingBEO model in StagePathDrawingList)
                {
                    isTrue = false;                   
                    string Qry = @"Insert Into WFM_StageMapDrawing(SourceStageId,TargetStageId,WorkflowId,SetEmployeeId,SetDate) 
                          Values(" + model.SourceStageId + "," + model.TargetStageId+ ",'" + model.WorkflowId + "','" + model.SetEmployeeId + "','" + CntDate + "')";

                    if (dbHelper.CmdExecute(dbConn.DocConnStrReader(), Qry))
                    {
                        IUMode = "I";
                        isTrue = true;
                    }
                }
                return isTrue;
            }
            catch (Exception errorException)
            {
                throw errorException;
            }
        }

        private void SaveNodeCoordinate(List<StageSetupBEO> stagePositionList)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ID");
            dt.Columns.Add("StageId");
            dt.Columns.Add("PositionX");
            dt.Columns.Add("PositionY");
            int i = 1;string vStageId = "";
            foreach (var item in stagePositionList)
            {
                vStageId = item.StageId;

                DataRow objDataRow = dt.NewRow();
                objDataRow[0] = i++;
                objDataRow[1] = item.StageId;
                objDataRow[2] = item.PositionX;
                objDataRow[3] = item.PositionY;
                dt.Rows.Add(objDataRow);
            }
            try
            {
                int cnt = dt.Rows.Count;
                DataTable vdtX = new DataView(dt).ToTable(true, "ID", "PositionX");
                DataRow[] draX = vdtX.Select("ID=" + cnt + "  ");

                DataTable tdtX = new DataTable();
                tdtX = vdtX.Clone();
                foreach (DataRow row in draX)
                {
                    tdtX.ImportRow(row);
                }

                DataTable vdtY = new DataView(dt).ToTable(true, "ID", "PositionY");
                DataRow[] draY = vdtY.Select("ID=" + cnt + "  ");

                DataTable tdtY = new DataTable();
                tdtY = vdtY.Clone();
                foreach (DataRow row in draY)
                {
                    tdtY.ImportRow(row);
                }

                string Qry = @"Update WFM_ProcessingStage Set PositionX="",PositionY="" Where StageId='"+ vStageId + "'";
                //DatabaseProviderFactory factory = new DatabaseProviderFactory();
                //SqlDatabase db = factory.CreateDefault() as SqlDatabase;
                //using (DbCommand dbCommandWrapper = db.GetStoredProcCommand("WFM_SaveNodeCoordinate"))
                //{
                //    // Set parameters 
                //    db.AddInParameter(dbCommandWrapper, "@WFM_DefinedProcessingStagePositions", SqlDbType.Structured, StagePositionListDataTable);
                //    db.AddInParameter(dbCommandWrapper, "@SetBy", SqlDbType.NVarChar, setBY);

                //    db.ExecuteNonQuery(dbCommandWrapper);
                //    _res_message = "Success";

                //}
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        internal object GetStageSetupList(string workflowId)
        {
            List<StageSetupBEO> list = null;
            string QryPostStage = @"Select StageId,StageName,StageSL,DefaultHaveMk,DefaultHaveCk,DefaultNotifyMk,DefaultNotifyCk,WorkflowId,SetEmployeeId,SetDate,IsActive
                                   from WFM_StageSetup Where WorkflowId='" + workflowId + "'  AND IsActive=1";
            DataTable dtPostStage = dbHelper.GetDataTable(dbConn.DocConnStrReader(), QryPostStage);
            IUMode = "PostStage";
            list = (from DataRow row in dtPostStage.Rows
                    select new StageSetupBEO
                    {
                        WorkflowId = row["WorkflowId"].ToString(),
                        // WorkflowName = row["WorkflowName"].ToString(),
                     
                        StageSL = row["StageSL"].ToString(),
                        StageId = row["StageId"].ToString(),
                        StageName = row["StageName"].ToString(),
                        HaveMk = Convert.ToBoolean(row["DefaultHaveMk"].ToString()),
                        HaveCk = Convert.ToBoolean(row["DefaultHaveCk"].ToString()),
                        IsActive = Convert.ToBoolean(row["IsActive"].ToString()),

                    }).ToList();
            return list;
        }
        internal object GetStagePathDrawingChart(string workflowId)
        {
            List<StagePathDrawingBEO> list = null;
            string QryPostStage = @"Select AID_StagePathDrawing,SourceStageId,TargetStageId,WorkflowId,SetEmployeeId,SetDate
                                   from WFM_StageMapDrawing Where WorkflowId='" + workflowId + "' ";
            DataTable dtPostStage = dbHelper.GetDataTable(dbConn.DocConnStrReader(), QryPostStage);
            IUMode = "PostStage";
            list = (from DataRow row in dtPostStage.Rows
                    select new StagePathDrawingBEO
                    {
                       // SL = row["AID_StagePathDrawing"].ToString(),
                        SourceStageId =Convert.ToInt16(row["SourceStageId"].ToString()),
                        TargetStageId = Convert.ToInt16(row["TargetStageId"].ToString()),
                        WorkflowId = row["WorkflowId"].ToString(),                
                
                  
                       

                    }).ToList();
            return list;
        }
        public List<StageSetupBEO> GetStageSetup(string workflowId,string stageId)
        {
            List<StageSetupBEO> list = null;
            string QryPostStage = @"Select StageId,StageName,StageSL,DefaultHaveMk,DefaultHaveCk,DefaultNotifyMk,DefaultNotifyCk,WorkflowId,SetEmployeeId,SetDate,IsActive
                                   from WFM_StageSetup Where WorkflowId='" + workflowId + "' AND StageId='"+ stageId + "' AND IsActive=1"; 
            DataTable dtPostStage = dbHelper.GetDataTable(dbConn.DocConnStrReader(), QryPostStage);
         

            list = (from DataRow row in dtPostStage.Rows
                    select new StageSetupBEO
                    {
                        WorkflowId = row["WorkflowId"].ToString(),
                        // WorkflowName = row["WorkflowName"].ToString(),
                  
                        StageSL = row["StageSL"].ToString(),
                        StageId = row["StageId"].ToString(),
                        StageName = row["StageName"].ToString(),
                        HaveMk = Convert.ToBoolean(row["DefaultHaveMk"].ToString()),
                        HaveCk = Convert.ToBoolean(row["DefaultHaveCk"].ToString()),
                        IsActive = Convert.ToBoolean(row["IsActive"].ToString()),

                    }).ToList();
            return list;
        }

        public bool Save(List<StageSetupBEO> modelList)
        {
            bool isTrue = false;
           
            string CntDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string setTerminal = terminal.GetLanIPAddress();
       
            try
            {
                foreach (StageSetupBEO model in modelList)
                {
                    isTrue = false;
                    model.IsActive = true;                   
                    string Qry = @"Insert Into WFM_StageSetup(StageName,StageSL,DefaultHaveMk,DefaultHaveCk,DefaultNotifyMk,DefaultNotifyCk,WorkflowId,SetEmployeeId,SetDate,IsActive) 
                          Values('" + model.StageName + "'," + model.StageSL + ",'" + (model.HaveMk == false ? '0' : '1') + "','" + (model.HaveCk == false ? '0' : '1') + "','" + (model.NotifyMk == false ? '0' : '1') + "','" + (model.NotifyCk == false ? '0' : '1') + "','" + model.WorkflowId + "','" + model.SetEmployeeId + "','" + CntDate + "'," + (model.IsActive == false ? '0' : '1') + ")";

                  

                    if (dbHelper.CmdExecute(dbConn.DocConnStrReader(), Qry))
                    {
                        string QryGet = "Select StageId from WFM_StageSetup Where StageName='" + model.StageName + "' And WorkflowId='"+ model.WorkflowId + "' ";
                        model.StageId = dbHelper.GetValue(dbConn.DocConnStrReader(), QryGet);
                        string QryStageMap = @"Insert Into WFM_StageMap(WorkflowId,StageId,StageMapSL,StageMapPosition,NextStage,HaveMk,HaveCk,NotifyMk,NotifyCk,SetEmployeeId,SetDate) 
                          Values('" + model.WorkflowId + "','" + model.StageId + "'," + model.StageSL + ",'"+model.StageMapPosition + "','" + model.NextStage + "','" + (model.HaveMk == false ? '0' : '1') + "','" + (model.HaveCk == false ? '0' : '1') + "','" + (model.NotifyMk == false ? '0' : '1') + "','" + (model.NotifyCk == false ? '0' : '1') + "','" + model.SetEmployeeId + "','" + CntDate + "')";
                      
                        dbHelper.CmdExecute(dbConn.DocConnStrReader(), QryStageMap);
                        IUMode = "I";
                        isTrue= true;                       
                    }
                }
                return isTrue;
            }
            catch (Exception errorException)
            {
                throw errorException;
            }
        }
        public bool Update(List<StageSetupBEO> modelList)
        {
            bool isTrue = false;
        
            string CntDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string setTerminal = terminal.GetLanIPAddress();

            try
            {
                foreach (StageSetupBEO model in modelList)
                {
                    isTrue = false;
                    model.IsActive = true;
                    string Qry = @"Update WFM_StageSetup Set StageName='" + model.StageName + "',DefaultHaveMk='" + (model.HaveMk == false ? '0' : '1') + "',DefaultHaveCk='" + (model.HaveCk == false ? '0' : '1') + "' Where StageSL=" + model.StageSL + " And WorkflowId='" + model.WorkflowId + "'"; 


                    if (dbHelper.CmdExecute(dbConn.DocConnStrReader(), Qry))
                    {
                        
                        IUMode = "U";
                        isTrue = true;
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