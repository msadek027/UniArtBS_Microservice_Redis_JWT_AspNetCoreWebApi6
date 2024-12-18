using Documents.Cache;
using Documents.Data;
using Documents.DocumentCommon;
using Documents.Models;
using Documents.Models.BEL;
using Documents.Models.DAL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Drawing.Drawing2D;

namespace Documents.Controllers
{
  
    // [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    [SwaggerControllerOrder(810)]
    public class Category_SubcategoryController : ControllerBase
    {
        TerminalLogger terminal = new TerminalLogger();
        DBConnection dbConn = new DBConnection();
        DBHelper dbHelper = new DBHelper();
        ExceptionHandler exceptionHandler = new ExceptionHandler();
        Category_SubcategoryDAO primaryDAO = new Category_SubcategoryDAO();
        private readonly DbContextClass _dbContext;
        private readonly ICacheService _cacheService;
        //https://www.c-sharpcorner.com/article/implementation-of-the-redis-cache-in-the-net-core-api/

        private readonly ILogger<Category_SubcategoryController> _logger;
        public Category_SubcategoryController(DbContextClass dbContext, ICacheService cacheService, ILogger<Category_SubcategoryController> logger)
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
            var cacheData = _cacheService.GetData<IEnumerable<Category_SubcategoryBEO>>("CategoryData");
            _cacheService.RemoveData("CategoryData");

            var cacheDataSub = _cacheService.GetData<IEnumerable<Category_SubcategoryBEO>>("SubcategoryData");
            _cacheService.RemoveData("SubcategoryData");
            isTrue = true;
            var responseCache = new ApiResponse<List<Category_SubcategoryBEO>>
            {
                Status = isTrue == true ? "Success" : "Fail",
                Message = isTrue == true ? "Execution Successful!" : "Execution Failed!",
                TotalPages = 0,
                Data = null
            };
            return Ok(responseCache);
        }
        [HttpPost]
        [Route("PostCategory")]
        public async Task<dynamic> SaveUpdateCategory(string CategoryId, string CategoryName)
        {
            bool isTrue = primaryDAO.SaveUpdateCategory(CategoryId, CategoryName);
            var items = new List<string>();
            items.Add(primaryDAO.MaxID);
            var response = new ApiResponse<List<string>>
            {
                Status = isTrue == true ? "Success" : "Fail",
                Message = isTrue == true ? "Execution Successful!" : "Execution Failed!",
                TotalPages = 0,
                Data = items
            };
            return Ok(response);
        }

        [HttpPost]
        [Route("PostSubcategory")]
        public async Task<dynamic> SaveUpdateSubcategory(string CategoryId, string SubcategoryId, string SubcategoryName)
        {
            bool isTrue = primaryDAO.SaveUpdateSubCategory(CategoryId, SubcategoryId, SubcategoryName);
            var items = new List<string>();
            items.Add(primaryDAO.MaxID);
            var response = new ApiResponse<List<string>>
            {
                Status = isTrue == true ? "Success" : "Fail",
                Message = isTrue == true ? "Execution Successful!" : "Execution Failed!",
                TotalPages = 0,
                Data = items
            };
            return Ok(response);
        }

        [HttpGet]
        [Route("GetCategoryList")]
        public ActionResult GetCategoryList()
        {
            bool isTrue = false;
            var data = primaryDAO.GetCategoryList();
            var rwCount = data == null ? 0 : data.Count;
            isTrue = true;
            var response = new ApiResponse<List<Category_SubcategoryBEO>>
            {
                Status = isTrue == true ? "Success" : "Fail",
                Message = isTrue == true ? "Execution Successful!" : "Execution Failed!",
                TotalPages = rwCount,
                Data = data
            };
            return Ok(response);
        }
        [HttpGet]
        [Route("GetSubcategoryList")]
        public ActionResult GetSubcategoryList()
        {
            bool isTrue = false;
            var data = primaryDAO.GetSubcategoryList();
            var rwCount = data == null ? 0 : data.Count;
            isTrue = true;
            var response = new ApiResponse<List<Category_SubcategoryBEO>>
            {
                Status = isTrue == true ? "Success" : "Fail",
                Message = isTrue == true ? "Execution Successful!" : "Execution Failed!",
                TotalPages = rwCount,
                Data = data
            };
            return Ok(response);
        }
   


    }
}
