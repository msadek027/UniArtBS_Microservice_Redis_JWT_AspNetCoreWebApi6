using Dapper;
using Documents.Cache;
using Documents.Data;
using Documents.DocumentCommon;
using Documents.Model;
using Documents.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace Documents.Controllers
{
   
   // [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    [SwaggerControllerOrder(802)]
    public class DocumentScanningSearchController : ControllerBase
    {
        TerminalLogger terminal = new TerminalLogger();
        DBConnection dbConn = new DBConnection();
        DBHelper dbHelper = new DBHelper();
        ExceptionHandler exceptionHandler = new ExceptionHandler();

        private readonly DbContextClass _dbContext;
        private readonly ICacheService _cacheService;
        //https://www.c-sharpcorner.com/article/implementation-of-the-redis-cache-in-the-net-core-api/

        private readonly ILogger<DocumentScanningSearchController> _logger;

        private static readonly DateTimeOffset expirationTime = DateTimeOffset.Now.AddMinutes(14400.0);//10 Days 
        public DocumentScanningSearchController(DbContextClass dbContext, ICacheService cacheService, ILogger<DocumentScanningSearchController> logger)
        {
            _dbContext = dbContext;
            _cacheService = cacheService;
            _logger = logger;

        }
        [HttpDelete]
        [Route("ResetRedisCache")]
        public async Task<dynamic> ResetRedisCache()
        {
            bool isTrue = false;          
            var cacheData = _cacheService.GetData<IEnumerable<DocumentScanningDataRedisCacheBEO>>("DocumentScanningData");
            _cacheService.RemoveData("DocumentScanningData");
            isTrue = true;         

            var responseCache = new ApiResponse<List<DocumentScanningDataRedisCacheBEO>>
            {
                Status = isTrue == true ? "Success" : "Fail",
                Message = isTrue == true ? "Execution Successful!" : "Execution Failed!",
                TotalPages =0,
                Data = null
            };
            return Ok(responseCache);
           
        }

        [HttpPost]
        [Route("PushDocumentScanningDataToRedisCache")]
        public IEnumerable<DocumentScanningDataRedisCacheBEO> Get()
        {
            bool isTrue = false;
            _cacheService.RemoveData("DocumentScanningData");
            var cacheData = _cacheService.GetData<IEnumerable<DocumentScanningDataRedisCacheBEO>>("DocumentScanningData");
            isTrue = true;
            if (cacheData != null)
            {
                return cacheData;
            }
            //var expirationTime = DateTimeOffset.Now.AddMinutes(14400.0);//10 Days
            // Use raw SQL with Dapper
            using (var connection = new SqlConnection(dbConn.DocConnStrReader()))
            {
                var Qry =@"Select ROW_NUMBER() OVER (ORDER BY A.DocumentId) AS SL,A.DocumentId,A.DocumentName,A.FileExtension,A.FileDirectory,A.SubcategoryId,A.SetEmployeeId,CONVERT(VARCHAR, A.SetDate, 23) As SetDate,A.IsActive, U.RoleId,
                        CASE When LEN(A.CompressedFileExtension)<2 Then A.FileExtension ELSE A.CompressedFileExtension END CalculativeExtension,A.CompressedFileExtension,A.FilePassword,
                        REPLACE(C.CategoryName+'/'+B.SubcategoryName, ' ', '') As LogicalFileDirectory,S.FtpServerIP,S.FtpPort,S.FtpUserId,S.FtpPassword,COUNT(*) OVER () AS TotalCount
                        from Doc_Documents As  A,Doc_CategorySub As B,Doc_Category As C,Doc_Document_FtpServer As S,dbSA.dbo.Sa_UserInRole U
                        Where A.SubcategoryId=B.SubcategoryId AND B.CategoryId=C.CategoryId AND A.DocumentId=S.DocumentId AND A.SetEmployeeId=U.EmpID AND A.IsActive=1";
               
                Qry = Qry + @" ORDER BY A.DocumentId DESC";
                // cacheData = connection.QueryFirstOrDefault<DocumentScanningDataRedisCacheBEO>(Qry, new { id });
                cacheData = connection.Query<DocumentScanningDataRedisCacheBEO>(Qry);
            }
            _cacheService.SetData<IEnumerable<DocumentScanningDataRedisCacheBEO>>("DocumentScanningData", cacheData, expirationTime);
            return cacheData;        
        }

        [HttpPost]
        [Route("GetDocuments")]
        public async Task<dynamic> GetDocuments([Required][RegularExpression(@"\d{4}-\d{2}-\d{2}", ErrorMessage = "FromDate must be in yyyy-MM-dd format.")] string FromDate, [Required][RegularExpression(@"\d{4}-\d{2}-\d{2}", ErrorMessage = "FromDate must be in yyyy-MM-dd format.")] string ToDate, string? documentId= null, int page=1, int itemsPerPage=10, string? searchAttribute= null)
        {
            int roleId = 1;// Convert.ToInt16(Session["RoleId"].ToString());
            bool isTrue = false;
            var items = new List<string>();
            List<DocumentScanningDataRedisCacheBEO> list = null;
            try
            {
                var cacheData = _cacheService.GetData<IEnumerable<DocumentScanningDataRedisCacheBEO>>("DocumentScanningData");
                if (cacheData != null)
                {                    
                     list = cacheData
                     .Where(x => DateTime.Parse(x.SetDate) >= DateTime.Parse(FromDate) &&
                                 DateTime.Parse(x.SetDate) <= DateTime.Parse(ToDate) &&
                                 (string.IsNullOrEmpty(documentId) || x.DocumentId == documentId) &&
                                 (string.IsNullOrEmpty(searchAttribute) || x.DocumentName.Contains(searchAttribute)))
                     .Skip((page - 1) * itemsPerPage)
                     .Take(itemsPerPage)
                     .ToList();                 

                    if(list.Count>0)
                    {
                        isTrue = true;
                        var responseCache = new ApiResponse<List<DocumentScanningDataRedisCacheBEO>>
                        {
                            Status = isTrue == true ? "Success" : "Fail",
                            Message = isTrue == true ? "Execution Successful!" : "Execution Failed!",
                            TotalPages = cacheData.Count(),
                            Data = list
                        };
                        return Ok(responseCache);
                    }                    
                }

                string Qry = @"DECLARE @page INT = " + page + ", @itemsPerPage INT = " + itemsPerPage + ";";
                Qry = Qry + @"Select ROW_NUMBER() OVER (ORDER BY A.DocumentId) AS Col1,A.DocumentId,A.DocumentName,A.FileExtension,A.FileDirectory,A.SubcategoryId,A.SetEmployeeId,A.SetDate,A.IsActive, A.SetDate,U.RoleId,
                        CASE When LEN(A.CompressedFileExtension)<2 Then A.FileExtension ELSE A.CompressedFileExtension END CalculativeExtension,A.CompressedFileExtension,A.FilePassword,
                        REPLACE(C.CategoryName+'/'+B.SubcategoryName, ' ', '') As LogicalFileDirectory,S.FtpServerIP,S.FtpPort,S.FtpUserId,S.FtpPassword,COUNT(*) OVER () AS TotalCount
                        from Doc_Documents As  A,Doc_CategorySub As B,Doc_Category As C,Doc_Document_FtpServer As S,dbSA.dbo.Sa_UserInRole U
                        Where A.SubcategoryId=B.SubcategoryId AND B.CategoryId=C.CategoryId AND A.DocumentId=S.DocumentId AND A.SetEmployeeId=U.EmpID AND A.IsActive=1";

                Qry = Qry + " And CAST(U.RoleId As Int)>=" + roleId + "";

                if (!string.IsNullOrEmpty(FromDate) && !string.IsNullOrEmpty(ToDate))
                {
                    Qry = Qry + " AND A.SetDate Between '" + FromDate + "' AND '" + ToDate + "'";
                }
                if (documentId != "" && documentId != null)
                {
                    Qry = Qry + " AND A.DocumentId='" + documentId + "'";
                }
                if (searchAttribute != "" && searchAttribute != null)
                {
                    Qry = Qry + " AND A.DocumentName LIKE '%" + searchAttribute + "%'";
                }
                Qry = Qry + @" ORDER BY A.DocumentId DESC
                        OFFSET ((@page - 1) * @itemsPerPage) ROWS FETCH NEXT @itemsPerPage ROWS ONLY;";


                DataTable dt = dbHelper.GetDataTable(dbConn.DocConnStrReader(), Qry);
                // DataTable dt = dbHelper.dtIncremented(dt2);
                var totalPages = dt.Rows.Count > 0 ? dt.Rows[0]["TotalCount"] : 0;
                isTrue= dt.Rows.Count > 0 ? true : false;
                list = (from DataRow row in dt.Rows
                        select new DocumentScanningDataRedisCacheBEO
                        {
                            SL = row["Col1"].ToString(),
                            DocumentId = row["DocumentId"].ToString(),
                            DocumentName = row["DocumentName"].ToString(),
                            FileExtension = row["FileExtension"].ToString(),
                            CompressedFileFormat = row["CompressedFileExtension"].ToString(),
                            CalculativeExtension = row["CalculativeExtension"].ToString(),
                            FilePassword = row["FilePassword"].ToString(),
                            FileDirectory = row["FileDirectory"].ToString(),
                            SubcategoryId = row["SubcategoryId"].ToString(),


                            FtpServerIP = row["FtpServerIP"].ToString(),
                            FtpPort = row["FtpPort"].ToString(),
                            FtpUserId = row["FtpUserId"].ToString(),
                            FtpPassword = row["FtpPassword"].ToString(),

                        }).ToList();
                //var expirationTime = DateTimeOffset.Now.AddMinutes(14400.0);//10 Days
                _cacheService.SetData<IEnumerable<DocumentScanningDataRedisCacheBEO>>("DocumentScanningData", list, expirationTime);
           
                var response = new ApiResponse<List<DocumentScanningDataRedisCacheBEO>>
                {
                    Status = isTrue == true ? "Success" : "Fail",
                    Message = isTrue == true ? "Execution Successful!" : "Execution Failed!",
                    TotalPages = totalPages,
                    Data = list
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = new ApiResponse<List<string>>
                {
                    Status = "Error",
                    Message = ex.Message,
                    TotalPages = 0,
                    Data = items
                };
                return Ok(response);
            }
        }



    }
}
