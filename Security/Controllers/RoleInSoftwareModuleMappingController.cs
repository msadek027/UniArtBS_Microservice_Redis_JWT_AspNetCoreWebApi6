using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Security.Models.BEL;
using Security.Models.DAL;
using Security.SecurityCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;



namespace Security.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    [SwaggerControllerOrder(106)]
    public class RoleInSoftwareModuleMappingController : ControllerBase
    {
        RoleInSoftwareModuleDAO primaryDAO = new RoleInSoftwareModuleDAO();
        ExceptionHandler exceptionHandler = new ExceptionHandler();
       
        [HttpGet]
        public JsonResult GetRoleInSoftwareModuleMappingList(string RoleID)
        {
            var model = primaryDAO.GetRoleInSoftwareModuleMappingList(RoleID);
            return new JsonResult(model);
        }

        [HttpPost]
        public JsonResult OperationsMode(RoleInSoftwareModuleBEL master)
        {
            try
            {
                if (primaryDAO.SaveUpdate(master))
                {
                    return new JsonResult(new { ID = primaryDAO.MaxID, Mode = primaryDAO.IUMode, Status = "Yes" });
                }
                else
                    return new JsonResult(new { Mode = "No", Status = "Yes" });
            }
            catch (Exception e)
            {
                return new JsonResult(exceptionHandler.ErrorMsg(e));
            }

        }
	}
}