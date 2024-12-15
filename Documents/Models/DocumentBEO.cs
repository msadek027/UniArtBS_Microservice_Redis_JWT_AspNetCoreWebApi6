using System.ComponentModel.DataAnnotations;

namespace Documents.Models
{
    public class DocumentBEO: FtpServerBEO
    {
             
            public string DocumentName { get; set; }
            public string CompressedFormat { get; set; }
            public string SubcategoryId { get; set; }
            public string FileDirectory { get; set; }
            public string SetEmployeeId { get; set; }

            public string FilePassword { get; set; }
            public IFormFile[] Files { get; set; }        

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

}
