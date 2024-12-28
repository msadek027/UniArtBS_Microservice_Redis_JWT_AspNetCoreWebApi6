using Documents.Cache;
using Documents.Data;
using Documents.DocumentCommon;
using Documents.Models.BEL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace Documents.Controllers
{
   // [ApiExplorerSettings(IgnoreApi = true)]
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentSharingController : ControllerBase
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
        public DocumentSharingController(DbContextClass dbContext, ICacheService cacheService)
        {
            _dbContext = dbContext;
            _cacheService = cacheService;
        }
        [HttpPost]
        [Route("SendMailWithDownloadLink")]
        public ActionResult SendMailWithDownloadLink(String toEmail, string ccAddress, string bccAddress, string Subj, string Message, List<DocumentSharingBEO> documentList)
        {
            string currentUrl = $"{Request.Scheme}://{Request.Host}{Request.Path}{Request.QueryString}";
            var items = new List<string>();
            bool isTrue = false;
            int a = 1;
            // HostAdd = ConfigurationManager.AppSettings["Host"].ToString();
            //// HostAdd = "172.16.128.42";
            // FromEmailid = ConfigurationManager.AppSettings["FromMail"].ToString();
            // Password = ConfigurationManager.AppSettings["Password"].ToString();

            string HostAdd = "smtp.gmail.com";
            // HostAdd = "172.16.128.42";
            string FromEmailid = "engr.msadek027@gmail.com";
            string Password = "ufkd zvot doer vftb";// "fgcc xclj zzqq bqrm";

            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(FromEmailid); //From Email Id
            mailMessage.Subject = Subj; //Subject of Email
            mailMessage.IsBodyHtml = true;
            string[] toMuliId = toEmail.Split(',');
            foreach (string toEMailId in toMuliId)
            {
                if (!string.IsNullOrEmpty(toEMailId))
                {
                    mailMessage.To.Add(new MailAddress(toEMailId)); //adding multiple TO Email Id
                }
            }
            if (ccAddress != "" && ccAddress != null)
            {
                string[] CCId = ccAddress.Split(',');
                foreach (string ccEmail in CCId)
                {
                    if (!string.IsNullOrEmpty(ccEmail))
                    {
                        mailMessage.CC.Add(new MailAddress(ccEmail)); //Adding Multiple CC email Id
                    }
                }
            }
            if (bccAddress != "" && bccAddress != null)
            {
                string[] bccid = bccAddress.Split(',');
                foreach (string bccEmailId in bccid)
                {
                    if (!string.IsNullOrEmpty(bccEmailId))
                    {
                        mailMessage.Bcc.Add(new MailAddress(bccEmailId)); //Adding Multiple BCC email Id
                    }
                }
            }

            string url;
            string ServerIP = documentList[0].FtpServerIP; string FileServerURL = documentList[0].FileDirectory; string FtpUserName = documentList[0].FtpUserId; string FtpPassword = documentList[0].FtpPassword;
            try
            {
                FtpWebRequest ftpWebRequest = (FtpWebRequest)FtpWebRequest.Create("ftp://" + ServerIP + "/" + FileServerURL + "/");
                ftpWebRequest.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                ftpWebRequest.Credentials = new NetworkCredential(FtpUserName, FtpPassword);
                ftpWebRequest.UsePassive = true;
                ftpWebRequest.UseBinary = true;
                ftpWebRequest.KeepAlive = false;
                List<DirectoryItem> returnValue = new List<DirectoryItem>();
                string[] list = null;
                List<string> finallist = new List<string>();
                using (FtpWebResponse ftpResponse = (FtpWebResponse)ftpWebRequest.GetResponse())           
            
                using (StreamReader reader1 = new StreamReader(ftpResponse.GetResponseStream()))
                    {
                        list = reader1.ReadToEnd().Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                        for (int i = 0; i < list.Length; i++)
                        {
                            string[] temporaryList = list[i].Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                            for (int j = 0; j < temporaryList.Length; j++)
                            {
                                if (temporaryList[j].Contains("."))
                                {
                                    finallist.Add(temporaryList[j]);
                                }
                            }
                        }
                    }
                

                foreach (var doc in documentList)
                {
                    for (int i = 0; i < finallist.Count; i++)
                    {
                        string tempDocumentID = doc.DocumentId + ".pdf";
                        if (finallist[i].Contains(tempDocumentID))
                        {
                            // Constructing the URL for API 6 with secure handling
                            string filePath = finallist[i];
                            string timeStamp = DateTime.Now.AddHours(24).ToString();
                            string encodedData = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{filePath}doctimexp{timeStamp}"));

                            // Building the full URL
                             url = $"{Request.Scheme}://{Request.Host}{Request.Path}'/DocumentSharing/DownloadLink?file={encodedData}";


                           
                          //  url = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/') + "/" + "DocumentSharing/DownloadLink?file=" + System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(finallist[i] + "doctimexp" + DateTime.Now.AddHours(24)));
                            mailMessage.Body = mailMessage.Body + Message + "<h4>To Download File go to the link below :</h4>" + "<p><a href=" + url + ">Download</a></p>" + "<p>N.B:This link is valid for 24 hours only</p>";
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                //continue;
                using (StreamWriter writer = new StreamWriter("C:\\Log\\log.txt", true))
                {
                    writer.WriteLine("FTP Method Call :" + ex.Message);
                }
            }
            SmtpClient smtp = new SmtpClient(); // creating object of smptpclient
            smtp.Host = HostAdd; //host of emailaddress for example smtp.gmail.com etc/  network and security related credentials
            smtp.EnableSsl = true;
            NetworkCredential networkCred = new NetworkCredential();
            networkCred.UserName = mailMessage.From.Address;
            networkCred.Password = Password;
            smtp.UseDefaultCredentials = true;
            smtp.Credentials = networkCred;
            // smtp.Port = Convert.ToInt32(ConfigurationManager.AppSettings["Port"]);
            smtp.Port = Convert.ToInt32("587");
            ServicePointManager.ServerCertificateValidationCallback = (s, certificate, chain, sslPolicyErrors) => true;

            try
            {
                smtp.Send(mailMessage);
                //ViewBag.LoggResult = "";
                //ViewBag.LoggAction = "Mail Sent";
                //ViewBag.LookupTable = "DSM_Documents";
                a = 2;
                SavedSharedData(documentList, FromEmailid, toEmail, ccAddress, bccAddress, Subj, Message);
            }
            catch (Exception ex)
            {
                a = 0;
                using (StreamWriter writer = new StreamWriter("C:\\Log\\log.txt", true))
                {
                    writer.WriteLine("During Main Send Method Call :" + ex.Message);
                }
            }
      
            var response = new ApiResponse<List<string>>
            {
                Status = isTrue == true ? "Success" : "Fail",
                Message = isTrue == true ? "Execution Successful!" : "Execution Failed!",
                Data = items
            };
            return Ok(response);
        }
        //[ApiExplorerSettings(IgnoreApi = true)]
        [Route("SavedSharedData")]
        [HttpGet]
        public bool SavedSharedData(List<DocumentSharingBEO> documentList, string FromEmailid, String toEmail, string ccAddress, string bccAddress, string Subj, string Message)
        {
            string currentUrl = $"{Request.Scheme}://{Request.Host}{Request.Path}{Request.QueryString}";
            bool isTrue = false;
         
            string CntDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string setTerminal = terminal.GetLanIPAddress();


            string Str = toEmail.Replace("[", "").Replace("]", "").Replace("\"", "'");
            string[] empLoop = Str.Replace("'", "").Split(',');

            string emp = toEmail;
            //for (int i = 0; i < empLoop.Length; i++)
            //{
            // string emp = empLoop[i];
            foreach (var doc in documentList)
            {
                string Qry = @"Insert Into Doc_Distribuion_Sharing(DocumentId,DocumentName,FileExtension,SubcategoryId,FileDirectory,SetEmployeeId,SetDate,IsActive,OperationName,DistribuionTo_SharingTo,MailBodyContext_Remarks,MailFrom,MailTo,MailCCTo,MailBCCTo,MailSubject) 
                              Values('" + doc.DocumentId + "','" + doc.DocumentName + "','" + doc.FileExtension + "','" + doc.SubcategoryId + "','" + doc.FileDirectory + "','" + doc.SetEmployeeId + "','" + CntDate + "',1,'Sharing','" + emp + "','" + Message + "','" + FromEmailid + "','" + emp + "','" + ccAddress + "','" + bccAddress + "','" + Subj + "')";

                if (dbHelper.CmdExecute(dbConn.DocConnStrReader(), Qry))
                {
                    isTrue = true;
                }
                terminal.OperationalLogTaleCallFromCtrl(doc.DocumentId, doc.SetEmployeeId, currentUrl, setTerminal, "DocumentId", "Doc_Distribuion_Sharing", "Insert");
                // }
            }
            return isTrue;
        }
        
        [Route("DownloadLink")]
        [HttpGet]
        public ActionResult DownloadLink(string file)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(file);
            file = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
            //file = StringEncription.Decrypt(file,true);
            string[] data = file.Split(new[] { "doctimexp" }, StringSplitOptions.None);
            DateTime ExpirationDate = DateTime.Parse(data[1]);
            if (ExpirationDate > DateTime.Now)
            {
                DocumentSharingBEO document = null;
                string[] filedata = data[0].Split(new[] { "." }, StringSplitOptions.None);
                // LOGSAVED
                using (WebClient request = new WebClient())
                {
                    request.Credentials = new NetworkCredential(document.FtpUserId, document.FtpPassword);
                    try
                    {
                        byte[] fileData = request.DownloadData("ftp://" + document.FtpServerIP + "/" + document.FileDirectory + "/" + data[0]);

                        var cd = new System.Net.Mime.ContentDisposition
                        {
                            FileName = data[0],
                            Inline = false,
                        };
                       // Response.AppendHeader("Content-Disposition", cd.ToString());
                        return File(fileData, "application / " + "." + filedata[1]);
                    }
                    catch (Exception e)
                    {
                        return RedirectToAction("BadRequest", "Home");
                    }
                }
            }
            else
            {
                return RedirectToAction("BadRequest", "Home");
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
        struct DirectoryItem
        {
            public Uri BaseUri;
            public string AbsolutePath
            {
                get
                {
                    return string.Format("{0}/{1}", BaseUri, Name);
                }
            }
            public DateTime DateCreated;
            public bool IsDirectory;
            public string Name;
            public List<DirectoryItem> Items;
        }
    }
}
