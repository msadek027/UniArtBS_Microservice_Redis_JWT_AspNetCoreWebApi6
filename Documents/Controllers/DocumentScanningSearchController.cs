using Documents.Cache;
using Documents.Data;
using Documents.DocumentCommon;
using Documents.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace Documents.Controllers
{
   
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    [SwaggerControllerOrder(602)]
    public class DocumentScanningSearchController : ControllerBase
    {
        Terminal terminal = new Terminal();
        DBConnection dbConn = new DBConnection();
        DBHelper dbHelper = new DBHelper();
        ExceptionHandler exceptionHandler = new ExceptionHandler();

        private readonly DbContextClass _dbContext;
        private readonly ICacheService _cacheService;
        //https://www.c-sharpcorner.com/article/implementation-of-the-redis-cache-in-the-net-core-api/

        //private static object _lock = new object();
        public DocumentScanningSearchController(DbContextClass dbContext, ICacheService cacheService)
        {
            _dbContext = dbContext;
            _cacheService = cacheService;

        }
        [HttpPost]
        [Route("GetDocuments")]
        public async Task<dynamic> GetDocuments([Required][RegularExpression(@"\d{4}-\d{2}-\d{2}", ErrorMessage = "FromDate must be in yyyy-MM-dd format.")] string FromDate, [Required][RegularExpression(@"\d{4}-\d{2}-\d{2}", ErrorMessage = "FromDate must be in yyyy-MM-dd format.")] string ToDate, string documentId, int page, int itemsPerPage, string searchAttribute)
        {
            //  string ftpServerIP = Session["FtpServerIP"].ToString(); string ftpPort = Session["FtpPort"].ToString(); string ftpUserId = Session["FtpUserId"].ToString(); string ftpPassword = Session["FtpPassword"].ToString();
            int roleId = 1;// Convert.ToInt16(Session["RoleId"].ToString());
            bool isTrue = false;
            var items = new List<string>();
            List<DocumentScanningBEO> list = null;
            try
            {
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
                if (documentId != "")
                {
                    Qry = Qry + " AND A.DocumentId='" + documentId + "'";
                }
                if (searchAttribute != "")
                {
                    Qry = Qry + " AND A.DocumentName LIKE '%" + searchAttribute + "%'";
                }
                Qry = Qry + @" ORDER BY A.DocumentId DESC
                        OFFSET ((@page - 1) * @itemsPerPage) ROWS FETCH NEXT @itemsPerPage ROWS ONLY;";
                DataTable dt = dbHelper.GetDataTable(dbConn.DocConnStrReader(), Qry);
                // DataTable dt = dbHelper.dtIncremented(dt2);
                var totalPages = dt.Rows.Count > 0 ? dt.Rows[0]["TotalCount"] : 0;
                list = (from DataRow row in dt.Rows
                        select new DocumentScanningBEO
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

                // items.Add(fileExt_fileId);
                var response = new ApiResponse<List<DocumentScanningBEO>>
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
