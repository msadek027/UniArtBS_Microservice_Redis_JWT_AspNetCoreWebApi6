using Documents.Cache;
using Documents.Data;
using Documents.DocumentCommon;
using Documents.Models.BEL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Reflection.Metadata;

namespace Documents.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentDistributionController : ControllerBase
    {
        TerminalLogger terminal = new TerminalLogger();
        DBConnection dbConn = new DBConnection();
        DBHelper dbHelper = new DBHelper();


        private readonly DbContextClass _dbContext;
        private readonly ICacheService _cacheService;
        //https://www.c-sharpcorner.com/article/implementation-of-the-redis-cache-in-the-net-core-api/

        private readonly ILogger<DocumentDistributionController> _logger;

        private static readonly DateTimeOffset expirationTime = DateTimeOffset.Now.AddMinutes(14400.0);//10 Days 

        //private static object _lock = new object();
        public DocumentDistributionController(DbContextClass dbContext, ICacheService cacheService)
        {
            _dbContext = dbContext;
            _cacheService = cacheService;
        }

        [HttpPost]
        [Route("PostDistribution")]  // [FromBody] [FromForm][FromRoute] [FromQuery] 
        public IActionResult SaveDistribution([FromForm] List<DocumentDistribution> documentList, [FromForm] List<DistributionToBEO> employeeList)
        {
            string currentUrl = $"{Request.Scheme}://{Request.Host}{Request.Path}{Request.QueryString}";
            bool isTrue = false;       
            string CntDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");    
            string setTerminal = Ip;
            var items = new List<string>();
            try
            {
                //string Str = employeeId.Replace("[", "").Replace("]", "").Replace("\"", "'");
                //string[] empLoop = Str.Replace("'", "").Split(',');
                foreach (var emp2 in employeeList)
                {
                    //for (int i = 0; i < empLoop.Length; i++)
                    //{
                    //    string emp = empLoop[i];
                    string emp = emp2.SetEmployeeId;
                        foreach (var doc in documentList)
                        {
                            string Qry = @"Insert Into Doc_Distribuion_Sharing(DocumentId,DocumentName,FileExtension,SubcategoryId,FileDirectory,SetEmployeeId,SetDate,IsActive,OperationName,DistribuionTo_SharingTo,MailBodyContext_Remarks) 
                              Values('" + doc.DocumentId + "','" + doc.DocumentName + "','" + doc.FileExtension + "','" + doc.SubcategoryId + "','" + doc.FileDirectory + "','" + doc.SetEmployeeId + "','" + CntDate + "',1,'Distribution','" + emp + "','" + doc.MailBodyContext_Remarks + "')";

                            if (dbHelper.CmdExecute(dbConn.DocConnStrReader(), Qry))
                            {
                                isTrue = true;
                            }
                            terminal.OperationalLogTaleCallFromCtrl(doc.DocumentId, doc.SetEmployeeId, currentUrl, setTerminal, "DocumentId", "Doc_Distribuion_Sharing", "Insert");
                            items.Add(doc.DocumentId);
                        }
                    //}
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
