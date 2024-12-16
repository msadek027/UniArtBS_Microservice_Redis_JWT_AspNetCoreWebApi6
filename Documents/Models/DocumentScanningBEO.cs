namespace Documents.Models
{
    public class DocumentScanningBEO : FtpServerBEO
    {
        public string SL { get; set; }
        public string DocumentId { get; set; }
        public string DocumentName { get; set; }
        public string FileDirectory { get; set; }
        public string SubcategoryId { get; set; }
        public string FileExtension { get; set; }
        public string CalculativeExtension { get; set; }
        public string CompressedFileFormat { get; set; }
        public string FilePassword { get; set; }
    }
}
