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
    [SwaggerControllerOrder(108)]
    public class UserInFormPermissionController : ControllerBase
    {
        UserInFormDAO primaryDAO = new UserInFormDAO();
       ExceptionHandler exceptionHandler = new ExceptionHandler();       

        [HttpPost]
        public JsonResult GetUserInFormPermissionList(string UserID)
        {
            var model = primaryDAO.GetUserInFormPermissionList(UserID);
            return new JsonResult(model);
        }
        [Route("PostUserInFormP")]
        [HttpPost]
        public JsonResult OperationsMode(UserInFormBEL master)
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