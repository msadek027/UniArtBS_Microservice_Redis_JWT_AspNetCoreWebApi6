using System.ComponentModel.DataAnnotations;

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
    public class DocumentScanningSingleFileBEO : FtpServerBEO
    {
        [Required(ErrorMessage = "Document Name is required.")]
        public string DocumentName { get; set; }

        [Required(ErrorMessage = "FileDirectory Name is required.")]
        public string FileDirectory { get; set; }

        [Required(ErrorMessage = "SubcategoryId Name is required.")]
        public string SubcategoryId { get; set; }

        [Required(ErrorMessage = "File is required.")]
        public IFormFile files { get; set; }
        public string? FilePassword { get; set; } = string.Empty;
        public string? CompressedFileFormat { get; set; } = string.Empty;
        public string? SetEmployeeId { get; set; } = string.Empty;

    }
    public class DocumentScanningMultipleFileBEO : FtpServerBEO
    {
        [Required(ErrorMessage = "Document Name is required.")]
        public string DocumentName { get; set; }
        [Required(ErrorMessage = "FileDirectory Name is required.")]
        public string FileDirectory { get; set; }
        [Required(ErrorMessage = "SubcategoryId Name is required.")]
        public string SubcategoryId { get; set; }

        [Required(ErrorMessage = "File is required.")]
        public IFormFile[] files { get; set; }

        public string? FilePassword { get; set; } = string.Empty;
        public string? CompressedFileFormat { get; set; } = string.Empty;
        public string? SetEmployeeId { get; set; } = string.Empty;

    }
    public class DocumentScanningInputBEO : FtpServerBEO
    {
        public string DocumentName { get; set; }
        public string FileDirectory { get; set; }
        public string SubcategoryId { get; set; }
        public string? FilePassword { get; set; }
        public string? CompressedFileFormat { get; set; }
        public string? SetEmployeeId { get; set; }
    }

    public class DocumentScanningDataRedisCacheBEO : FtpServerBEO
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

        public string SetDate { get; set; }
    }
}
