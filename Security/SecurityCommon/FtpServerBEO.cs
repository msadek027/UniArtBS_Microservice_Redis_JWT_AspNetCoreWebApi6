namespace Security.WorkflowCommon
{
    public class FtpServerBEO
    {
        public string? FtpServerIP { get; set; } = string.Empty;
        public string? FtpPort { get; set; } = string.Empty;
        public string? FtpUserId { get; set; } = string.Empty;
        public string? FtpPassword { get; set; } = string.Empty;

    }
    public class FtpServerRunningBEO : FtpServerBEO
    {
        public bool? IsRunning { get; set; }
    }
    public class ReturnData
    {
        public string MaxID { get; set; }
        public string MaxCode { get; set; }
        public string IUMode { get; set; }
    }
}
