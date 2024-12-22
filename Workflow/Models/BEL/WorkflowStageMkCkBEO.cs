using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Workflow.Models.BEL
{
    public class WorkflowStageMkCkBEO
    {
        public ICollection<StageDocumentTagBEO> Documents { get; set; }
        public ICollection<DocumentAttributeTagBEO> TypeProperties { get; set; }
        public ICollection<VM_ListTypeProperties> ListTypeProperties { get; set; }

    
    }
    public class WorkflowStageTagValueMkCkBEO
    {
        public ICollection<StageDocumentTagValueBEO> documentList { get; set; }
        public ICollection<StageDocumentTagValueBEO> updateDocuments { get; set; }
        public ICollection<DocumentAttributeTagValueBEO> attributeList { get; set; }
        public ICollection<VM_ListTypeProperties> ListTypeProperties { get; set; }
        public bool IsBacked { get;  set; }
        public string BackReason { get;  set; }

    
    }
    public class WorkflowStageDocumentAttributeReportBEO
    {
        public WorkflowCreationBEO workflow { get; set; }
        public List<StageDocumentTagValueBEO> documentList { get; set; }
        public List<DocumentAttributeTagValueBEO> attributeList { get; set; }

        public List<StageChangeLogHistoryBEO> stageChangeLogHistoryList { get; set; }
     
    }
    public class StageChangeLogHistoryBEO
    {
        public string FromStage { get; set; }
        public string ToStage { get; set; }
        public string SetEmployeeName { get; set; }
        public string SetDate { get; set; }
    }


    }