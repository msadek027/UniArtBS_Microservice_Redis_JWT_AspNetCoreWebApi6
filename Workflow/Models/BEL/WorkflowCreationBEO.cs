using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Workflow.Models.BEL
{
    public class WorkflowCreationBEO
    {
        public string SL { get; set; }
        public string WorkflowId { get; set; }
        public string WorkflowName { get;  set; }
        public string NumberOfStage { get;  set; }
        public string Remarks { get;  set; }
        public bool IsActive { get;  set; }
      
        public string CategoryId { get;  set; }
        public string FileDirectory { get;  set; }
        public string CategoryName { get;  set; }
        public string SetEmployeeId { get; set; }
    }
}