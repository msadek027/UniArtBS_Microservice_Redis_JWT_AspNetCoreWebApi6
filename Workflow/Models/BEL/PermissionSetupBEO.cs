using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Workflow.Models.BEL
{
    public class PermissionSetupBEO
    {
        public string WorkflowId { get;  set; }
        public string WorkflowName { get;  set; }
        public string StageSL { get;  set; }
        public string StageId { get;  set; }
        public string StageName { get;  set; }
       
        public bool IsActive { get;  set; }
    }
}