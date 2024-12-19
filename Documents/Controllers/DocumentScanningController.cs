using Dapper;
using Documents.Cache;
using Documents.Data;
using Documents.DocumentCommon;
using Documents.Model;
using Documents.Models.BEL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Data.Common;
using System.Net;
using System.Net.Mail;
using System.Net.Sockets;


namespace Documents.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    [SwaggerControllerOrder(801)]
    public class DocumentScanningController : ControllerBase
    {
        TerminalLogger terminal = new TerminalLogger();
        DBConnection dbConn = new DBConnection();
        DBHelper dbHelper = new DBHelper();
            

        private readonly DbContextClass _dbContext;
        private readonly ICacheService _cacheService;
        //https://www.c-sharpcorner.com/article/implementation-of-the-redis-cache-in-the-net-core-api/

        //private static object _lock = new object();
        public DocumentScanningController(DbContextClass dbContext, ICacheService cacheService)
        {
            _dbContext = dbContext;
            _cacheService = cacheService;           
        }
       

        [HttpPost]
        [Route("DocUploadSingleFile")]      
        public async Task<IActionResult> DocUploadSingleFile([FromForm] DocumentScanningSingleFileBEO obj)
        {
            string currentUrl = $"{Request.Scheme}://{Request.Host}{Request.Path}{Request.QueryString}";
            string macAddress = "A08CFDE4D7F7";// HttpContext.Connection.RemoteIpAddress?.ToString();
            obj.SetEmployeeId = "11111";
            bool isTrue = false;
            var items = new List<string>();         
      
            try
            {
                //var terminalHelper = new HelperController();
                string setTerminal = GetIP();
                //string setTerminal2 = GetClientIp();
                
                string CntDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");               
                string formattedDateTime = DateTime.Now.ToString("yyyyMMdd-HHmmss-fff");
                string originalFile = obj.files.FileName;
                string[] parts = originalFile.Split('.');
                string fileNameWithoutExtension = string.Join(".", parts[..^1]);
                int countHyphen = fileNameWithoutExtension.Count(c => c == '-');
                int lastDotIndex = originalFile.LastIndexOf('.');
                string FileExtension = lastDotIndex != -1? originalFile.Substring(lastDotIndex + 1): string.Empty;
                string DocumentId = countHyphen>=3 ? fileNameWithoutExtension : formattedDateTime + "-" + macAddress;
                string fileExt_fileId = DocumentId + "." + FileExtension;
                var filePath = obj.FileDirectory + "//" + fileExt_fileId;
                if (DocumentId.Contains("-V"))
                {
                    string[] tempDocumentId = DocumentId.Split(new string[] { "-V" }, StringSplitOptions.None);
                    DocumentId = tempDocumentId[0];
                    string QryMaxLen = @"Select MAX(LEN(DocumentId)) from Doc_Documents Where DocumentId Like '%" + DocumentId + "%'";
                    string MaxLen = dbHelper.GetValue(dbConn.DocConnStrReader(), QryMaxLen);
                    string QryMaxId = @"Select MAX(CAST(Substring(DocumentId,35," + MaxLen + ") AS INT))+1 from  Doc_Documents Where DocumentId Like '%" + DocumentId + "%'";

                    string MaxId = dbHelper.GetValue(dbConn.DocConnStrReader(), QryMaxId);
                    DocumentId = DocumentId + "-V" + MaxId;
                    filePath = obj.FileDirectory + "//" + DocumentId + "." + FileExtension;
                }
                fileExt_fileId = DocumentId + "." + FileExtension;
                string Qry = @"Insert Into Doc_Documents(DocumentId,DocumentName,FileExtension,SubcategoryId,FileDirectory,SetEmployeeId,IsActive,FilePassword,CompressedFileExtension,SetDate) 
                              Values('" + DocumentId + "','" + obj.DocumentName + "','" + FileExtension + "','" + obj.SubcategoryId + "','" + obj.FileDirectory + "','" + obj.SetEmployeeId + "',1,'" + obj.FilePassword + "','" + obj.CompressedFileFormat + "','" + CntDate + "')";
                isTrue = dbHelper.CmdExecute(dbConn.DocConnStrReader(), Qry);
                terminal.OperationalLogTaleCallFromCtrl(DocumentId, obj.SetEmployeeId, currentUrl, setTerminal, "DocumentId", "Doc_Documents", "Insert");


                DocumentScanningInputBEO tempObj = new DocumentScanningInputBEO();
                tempObj.DocumentName = obj.DocumentName;
                tempObj.FileDirectory = obj.FileDirectory;
                tempObj.SubcategoryId = obj.SubcategoryId;
                tempObj.FilePassword = obj.FilePassword;
                tempObj.CompressedFileFormat = obj.CompressedFileFormat;
                tempObj.SetEmployeeId = obj.SetEmployeeId;

                tempObj.FtpServerIP = obj.FtpServerIP;
                tempObj.FtpPort = obj.FtpPort;
                tempObj.FtpUserId = obj.FtpUserId;
                tempObj.FtpPassword = obj.FtpPassword;

                var fileDownloadController = new FileUploadController();
                fileDownloadController.DocUploadSingleFile(tempObj, fileExt_fileId, obj.files, "DocumentId", "Doc_Documents");

                items.Add(fileExt_fileId);
                var response = new ApiResponse<List<string>>
                {
                    Status = isTrue==true?"Success":"Fail",
                    Message = isTrue == true ? "Execution Successful!" : "Execution Failed!",
                    TotalPages=1,
                    Data = items
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = new ApiResponse<List<string>>
                {
                    Status =  "Error",
                    Message = ex.Message,
                    TotalPages = 0,
                    Data = items
                };
                return Ok(response);
            }
           
        }
        [HttpPost]
        [Route("DocUploadMultipleFile")]
        public async Task<dynamic> DocUploadMultipleFile([FromForm] DocumentScanningMultipleFileBEO obj)
        {
            

            string currentUrl = $"{Request.Scheme}://{Request.Host}{Request.Path}{Request.QueryString}";
            string macAddress = "A08CFDE4D7F7";//obj.SetEmployeeId = "11111".PadLeft(12, '0');output: 00000011111
            obj.SetEmployeeId = "11111";
            bool isTrue = false;
            var items = new List<string>();
            try
            {
               // var terminalHelper = new HelperController();
                string setTerminal = Ip;            

                foreach (var file in obj.files)
                {

                    string formattedDateTime = DateTime.Now.ToString("yyyyMMdd-HHmmss-fff");
                    string CntDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                    string originalFile = file.FileName;
                    string[] parts = originalFile.Split('.');
                    string fileNameWithoutExtension = string.Join(".", parts[..^1]);
                    int countHyphen = fileNameWithoutExtension.Count(c => c == '-');
                    int lastDotIndex = originalFile.LastIndexOf('.');
                    string FileExtension = lastDotIndex != -1 ? originalFile.Substring(lastDotIndex + 1) : string.Empty;
                    string DocumentId = countHyphen>=3 ? fileNameWithoutExtension : formattedDateTime + "-" + macAddress;
                    string fileExt_fileId = DocumentId + "." + FileExtension;
                    var filePath = obj.FileDirectory + "//" + fileExt_fileId;
                    if (DocumentId.Contains("-V"))
                    {
                        string[] tempDocumentId = DocumentId.Split(new string[] { "-V" }, StringSplitOptions.None);
                        DocumentId = tempDocumentId[0];
                        string QryMaxLen = @"Select MAX(LEN(DocumentId)) from Doc_Documents Where DocumentId Like '%" + DocumentId + "%'";
                        string MaxLen = dbHelper.GetValue(dbConn.DocConnStrReader(), QryMaxLen);
                        string QryMaxId = @"Select MAX(CAST(Substring(DocumentId,35," + MaxLen + ") AS INT))+1 from  Doc_Documents Where DocumentId Like '%" + DocumentId + "%'";

                        string MaxId = dbHelper.GetValue(dbConn.DocConnStrReader(), QryMaxId);
                        DocumentId = DocumentId + "-V" + MaxId;
                        filePath = obj.FileDirectory + "//" + DocumentId + "." + FileExtension;
                    }
                    fileExt_fileId = DocumentId + "." + FileExtension;
                    string Qry = @"Insert Into Doc_Documents(DocumentId,DocumentName,FileExtension,SubcategoryId,FileDirectory,SetEmployeeId,IsActive,FilePassword,CompressedFileExtension,SetDate) 
                              Values('" + DocumentId + "','" + obj.DocumentName + "','" + FileExtension + "','" + obj.SubcategoryId + "','" + obj.FileDirectory + "','" + obj.SetEmployeeId + "',1,'" + obj.FilePassword + "','" + obj.CompressedFileFormat + "','" + CntDate + "')";
                    isTrue = dbHelper.CmdExecute(dbConn.DocConnStrReader(), Qry);

                    terminal.OperationalLogTaleCallFromCtrl(DocumentId, obj.SetEmployeeId, currentUrl, setTerminal, "DocumentId", "Doc_Documents", "Insert");


                    DocumentScanningInputBEO tempObj = new DocumentScanningInputBEO();
                    tempObj.DocumentName = obj.DocumentName;
                    tempObj.FileDirectory = obj.FileDirectory;
                    tempObj.SubcategoryId = obj.SubcategoryId;
                    tempObj.FilePassword = obj.FilePassword;
                    tempObj.CompressedFileFormat = obj.CompressedFileFormat;
                    tempObj.SetEmployeeId = obj.SetEmployeeId;

                    tempObj.FtpServerIP = obj.FtpServerIP;
                    tempObj.FtpPort = obj.FtpPort;
                    tempObj.FtpUserId = obj.FtpUserId;
                    tempObj.FtpPassword = obj.FtpPassword;
                    items.Add(fileExt_fileId);
                    var fileDownloadController = new FileUploadController();
                    fileDownloadController.DocUploadSingleFile(tempObj, fileExt_fileId, file, "DocumentId", "Doc_Documents");
                }
               
                var response = new ApiResponse<List<string>>
                {
                    Status = isTrue == true ? "Success" : "Fail",
                    Message = isTrue == true ? "Execution Successful!" : "Execution Failed!",
                    Data = items
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = new ApiResponse<List<string>>
                {
                    Status = "Error",
                    Message = ex.Message,
                    Data = items
                };
                return Ok(response);
            }
            
        }
      
        public string Ip
        {
            get
            {
                var remoteIp = HttpContext.Connection.RemoteIpAddress;
                if (remoteIp != null && remoteIp.IsIPv4MappedToIPv6)
                {
                    return remoteIp.MapToIPv4().ToString();
                }
                else return remoteIp.ToString();
            }
        }
        private string GetIP()
        {
            string strHostName = "";
            strHostName = System.Net.Dns.GetHostName();
            IPHostEntry ipEntry = System.Net.Dns.GetHostEntry(strHostName);
            IPAddress[] addr = ipEntry.AddressList;
            return addr[addr.Length - 1].ToString();
        }
      
    }
}
