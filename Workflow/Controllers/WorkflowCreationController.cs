using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using Workflow.Models.BEL;
using Workflow.Models.DAL;
using Workflow.WorkflowCommon;

namespace Workflow.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkflowCreationController : ControllerBase
    {
        WorkflowCreationDAO primaryDAO = new WorkflowCreationDAO();
        //ExceptionHandler exceptionHandler = new ExceptionHandler();

        [HttpPost]
        [Route("PostWorkflow")]
        public async Task<dynamic> SaveUpdate(WorkflowCreationBEO model)
        {
            try
            {
                bool isTrue = false;
                var items = new List<string>();
                if (primaryDAO.SaveUpdate(model))
                {
                    isTrue=true;
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
                    Data = null
                };
                return Ok(response);
            }
        }
        [HttpGet]
        [Route("GetWorkflowList")]
        public async Task<dynamic> GetWorkflowList(string? CategoryId=null, int page=1, int itemsPerPage=10, string? searchAttribute = null)
        {
            bool isTrue = false;
            try
            {
                var items = new List<string>();
            var tuple = primaryDAO.GetWorkflowList(CategoryId, page, itemsPerPage, searchAttribute);

            var totalPages = tuple.Item1;
            DataTable dt = tuple.Item2;
            var list = tuple.Item3;
                var rwCount = list == null ? 0 : list.Count;
                var response = new ApiResponse<List<WorkflowCreationBEO>>
                {
                    Status = isTrue == true ? "Success" : "Fail",
                    Message = isTrue == true ? "Execution Successful!" : "Execution Failed!",
                    TotalPages = rwCount,
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
                    Data = null
                };
                return Ok(response);
            }
        }
    }
}
