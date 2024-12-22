using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Workflow.Models.BEL;
using Workflow.WorkflowCommon;

namespace Workflow.Models.BEL
{
    public class StageSetupBEO
    {
        public string StageSL { get;  set; }
        public string StageId { get; set; }
        public string StageName { get;  set; }        
        public bool HaveMk { get;  set; }
        public bool HaveCk { get;  set; }     
        public bool NotifyMk { get;  set; }
        public bool NotifyCk { get;  set; }
        public bool IsActive { get; set; }
        public string CategoryId { get; set; }
        public string WorkflowId { get; set; }
        public int NumberOfStage { get; set; }
        public int PositionX { get; set; }
        public int PositionY { get; set; }
        public string StageMapPosition { get;  set; }
        public string NextStage { get;  set; }

        public string SetEmployeeId { get; set; }
    }
    public class StagePathDrawingBEO
    {
        public int StageConnectionId { get; set; }
        public int SourceStageId { get; set; }
        public int TargetStageId { get; set; }

        public string CategoryId { get; set; }
        public string WorkflowId { get; set; }


        public string SetEmployeeId { get; set; }
    }
    public class StageSetupStageMapBEO
    {
        public string StageMapId { get; set; }
        public string StageId { get; set; }
        public string StageName { get; set; }
        public bool IsChecked { get; set; }

    }
    public class DocumentAttributeTagBEO
    {
        public string AttributeTagId { get; set; }
        public string AttributeTagName { get; set; }
        public string AttributeTagType { get; set; }
        public string AttributeTagSL { get; set; }
         public bool IsRequired { get; set; }      
        public string StageId { get; set; }
        public string WorkflowId { get; set; }
        public string CategoryId { get; set; }
        public string StageMapId { get; set; }
        public string OpMode { get; set; }
        public bool IsActive { get;  set; }
        public string SetEmployeeId { get; set; }
        public List<WFM_TableProperty> TableProperties { get; set; }

    }
    public class DocumentAttributeTagValueBEO: DocumentAttributeTagBEO
    {
        public string AttributeTagValue { get; set; }
        public string ObjectId { get;  set; }
    }
    public class VM_ListTypeProperties
    {
        public List<WFM_TableProperty> ColumnList { get; set; }
        public string TableRefID { get; set; }
        public string AttributeName { get; set; }
    }
    public class WFM_TableProperty
    {
        public string FieldTitle { get; set; }
        public string DataType { get; set; }
        public string Value { get; set; }
        public string Master { get; set; }
        public int MaxSize { get; set; }
        public int RelationId { get; set; }
    }
    public class StageDocumentTagBEO:FtpServerBEO
    {
        public string DocumentTagId { get; set; }
        public string DocumentTagName { get; set; }
        public int DocumentTagSL { get; set; }
        public string IsRequired { get; set; }
        public string StageId { get; set; }      
        public string WorkflowId { get; set; }
        public string CategoryId { get; set; }
   
        public bool IsActive { get;  set; }
     
        public bool IsSelected { get; set; }
        public string SetEmployeeId { get; set; }

    }
    public class StageDocumentTagValueBEO: StageDocumentTagBEO
    {
        public string DocumentTagValue { get; set; }
        public string ObjectId { get;  set; }
        public bool IsBacked { get;  set; }
        public string BackReason { get;  set; }
        public string StageName { get;  set; }
        public string FileDirectory { get; set; }


  


    }
    public class DocumentTagPermission: StageSetupBEO    {
     
        public bool IsChecked { get; set; }
    }

    public class FileDocumentBEO
    {
        public string FileId { get; set; }
        public string FileName { get; set; }
        public string FileDirectoryId { get; set; }
        public string FileDirectoryName { get; set; }

    }
    public class ObjectBEO
    {
        public string ObjectId { get; set; }
        public string WorkflowId { get; set; }
        public string StageId { get; set; }

        public string SetEmployeeId { get; set; }
        public string CurrentURL { get; set; }
        public string SetTermial { get; set; }

    }
    public class ObjectNextStageBEO
    {
        public string ObjectId { get; set; }
        public string WorkflowId { get; set; }
        public string StageId { get; set; }
        public bool IsBack { get;  set; }
        public bool IsMakeCheck { get;  set; }
    }
    public class StageUserMappingBEO
    {
        public string ObjectId { get; set; }
        public string WorkflowId { get; set; }
        public string StageId { get; set; }
       
        public string StageSL { get; set; }
        public string StageName { get;  set; }
        public string WorkflowName { get;  set; }
        public string CategoryName { get; set; }

        public bool IsFirstStageUser { get;  set; }
        public bool IsIntermediateStageUser { get;  set; }
        public bool IsFinalStageUser { get;  set; }
   
    }
}