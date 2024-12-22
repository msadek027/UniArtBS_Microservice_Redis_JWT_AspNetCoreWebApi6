using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Workflow.Models.BEL;
using Workflow.Models.DAL;
using Workflow.WorkflowCommon;

namespace Workflow.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StageSetupController : ControllerBase
    {
        StageSetupDAO primaryDAO = new StageSetupDAO();
        [HttpGet]
        [Route("GetGenerateStageSetupList")]
        public async Task<dynamic> GetGenerateStageSetupList(string workflowId)
        {
            try
            {
                bool isTrue = false;
                var items = new List<string>();
                var list = primaryDAO.GetGenerateStageSetupList(workflowId);
                var response = new ApiResponse<List<StageSetupBEO>>
                {
                    Status = isTrue == true ? "Success" : "Fail",
                    Message = isTrue == true ? "Execution Successful!" : "Execution Failed!",
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
                    Data = null
                };
                return Ok(response);
            }
        }
        [HttpGet]
        [Route("GetStageSetup")]
        public async Task<dynamic> GetStageSetup(string workflowId, string stageId)
            {
                try
                {
                    bool isTrue = false;
                    var items = new List<string>();
                    var list = primaryDAO.GetStageSetup(workflowId, stageId);

                var response = new ApiResponse<List<StageSetupBEO>>
                {
                    Status = isTrue == true ? "Success" : "Fail",
                    Message = isTrue == true ? "Execution Successful!" : "Execution Failed!",
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
                    Data = null
                };
                return Ok(response);
            }
        }
        [HttpPost]
        [Route("PostStageSetup")]
        public async Task<dynamic> Save(List<StageSetupBEO> model)
        {
            bool isTrue = false;
            var items = new List<string>();
            try
            {
               

                if (primaryDAO.Save(model))
                {
                    isTrue = true;
                    items.Add(primaryDAO.MaxID);
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
        [HttpPost]
        [Route("PostStageSetupUpdate")]
        public async Task<dynamic> Update(List<StageSetupBEO> model)
        {
            bool isTrue = false;
            var items = new List<string>();
            try
            {
               
                if (primaryDAO.Update(model))
                {
                    items.Add(primaryDAO.MaxID);
                    isTrue = true;
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


    }
}
