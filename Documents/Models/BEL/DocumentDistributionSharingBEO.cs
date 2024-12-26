namespace Documents.Models.BEL
{
    public class DocumentDistributionSharingBEO: DocumentDistribution
    {
        public string MailBodyContext_Remarks { get; set; }
        //public string OperationName { get; set; }
        //public string Who { get; set; }
        //public string Whom { get; set; }
      //  public string DistribuionTo_SharingTo { get; set; }

        public string SetEmployeeId { get; set; } 
    }
    public class DocumentDistribution
    {
        public string DocumentId { get; set; }
        public string DocumentName { get; set; }
        public string FileDirectory { get; set; }
        public string SubcategoryId { get; set; }
        public string FileExtension { get; set; }
        public string MailBodyContext_Remarks { get; set; }
        public string SetEmployeeId { get; set; }
    }

    public class DistributionToBEO 
    { 
        public string SetEmployeeId { get; set; }
    }
    public class DocumentSharingBEO : DocumentScanningBEO
    {
        public string MailBodyContext_Remarks { get; set; }
        public string OperationName { get; set; }
        public string Who { get; set; }
        public string Whom { get; set; }
        public string DistribuionTo_SharingTo { get; set; }
        public string SetEmployeeId { get; set; }
    }

}
