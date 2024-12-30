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
    [SwaggerControllerOrder(100)]
    public class SoftwareController : ControllerBase
    {
        SoftwareDAO primaryDAO = new SoftwareDAO();
       ExceptionHandler exceptionHandler = new ExceptionHandler();      


        [HttpGet]
        public JsonResult GetSoftware()
        {
            var data = primaryDAO.GetSoftwareList();
            return new JsonResult(data );
        }



        [HttpPost]
        public JsonResult OperationsMode(SoftwareBEL master)
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