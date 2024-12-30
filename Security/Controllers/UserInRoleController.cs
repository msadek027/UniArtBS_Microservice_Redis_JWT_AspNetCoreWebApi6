using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Security.Models.BEL;
using Security.Models.DAL;
using Security.SecurityCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Web;


namespace Security.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    [SwaggerControllerOrder(104)]
    public class UserInRoleController : ControllerBase
    {
        UserInRoleDAO primaryDAO = new UserInRoleDAO();
        ExceptionHandler exceptionHandler = new ExceptionHandler();

        [Route("Sa/UserInRole/GetEmployee")]
        [HttpGet]
        public JsonResult GetEmployee()
        {
            var data = primaryDAO.GetEmployeeList();
            return new JsonResult(data);
        }
        [Route("Sa/UserInRole/GetUser")]
        [HttpGet]
        public JsonResult GetUser()
        {
            var data = primaryDAO.GetUserList();
            return new JsonResult(data);
        }
        [Route("Sa/UserInRole/GetRoleInRole")]
        [HttpGet]
        public JsonResult GetUserInRole(string sessionRoleId)
        {
            var data = primaryDAO.GetUserInRoleList(sessionRoleId);
            return new JsonResult(data);
        }
        [Route("Sa/UserInRole/GetEmpYetAssigned")]
        [HttpPost]
        public JsonResult GetEmpYetAssigned(string RoleID)
        {
            var data = primaryDAO.GetEmpYetAssignedList(RoleID);
            return new JsonResult(data);
        }

        [Route("Sa/UserInRole/PostUserInRole")]
        [HttpPost]
        public JsonResult OperationsMode(UserInRoleBEL master)
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