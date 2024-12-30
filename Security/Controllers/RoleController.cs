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
    [SwaggerControllerOrder(103)]
    public class RoleController : ControllerBase
    {

        RoleDAO primaryDAO = new RoleDAO();
        ExceptionHandler exceptionHandler = new ExceptionHandler();


        [HttpGet]
        public JsonResult GetRole()
        {
            var data = primaryDAO.GetRoleList();
            return new JsonResult(data);
        }

        //[HttpGet]
        //public JsonResult GetRole()
        //{
        //    var data = primaryDAO.GetDataTableRole();
        //    return new JsonResult(data);
        //}


        [HttpPost]
        public JsonResult OperationsMode(RoleBEL master)
        {
            try
            {
                if (primaryDAO.SaveUpdate(master))
                {
                    //return new JsonResult("Added Successfully");
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