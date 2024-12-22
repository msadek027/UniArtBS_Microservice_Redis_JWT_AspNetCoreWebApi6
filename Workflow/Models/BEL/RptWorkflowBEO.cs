using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Workflow.Models.BEL;
using Workflow.WorkflowCommon;

namespace Workflow.Models.BEL
{
    public class RptWorkflowBEO:FtpServerBEO
    {
        public string SL { get;  set; }
        public string ObjectId { get;  set; }
        public string DocumentId { get;  set; }
        public string DocumentTagValue { get;  set; }
        public string FileServerURL { get;  set; }
        public string FileDirectory { get;  set; }
    }
}