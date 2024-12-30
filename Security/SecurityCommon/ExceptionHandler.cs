using Microsoft.AspNetCore.Mvc;

namespace Security.SecurityCommon
{
    public class ExceptionHandler :ControllerBase
    {
        public JsonResult ErrorMsg(Exception e)
        {
            if (e.Message.Substring(0, 9) == "ORA-00001")
                return new JsonResult(new { Status = "Error: ORA-00001,Data already exists!" });//Unique Identifier.
            else if (e.Message.Substring(0, 9) == "ORA-02292")
                return new JsonResult(new { Status = "Error: ORA-02292,Data already exists!" });//Child Record Found.
            else if (e.Message.Substring(0, 9) == "ORA-12899")
                return new JsonResult(new { Status = "Error: ORA-12899,Data Value Too Large!" });//Value Too Large.
            else
                return new JsonResult(new { Status = "! Error: " + e.Message.Substring(0, 9) });//Other Wise Error Found
        }
    }
}
