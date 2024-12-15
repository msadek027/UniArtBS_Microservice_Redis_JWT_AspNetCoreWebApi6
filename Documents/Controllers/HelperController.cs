using Microsoft.AspNetCore.Mvc;

namespace Documents.Controllers
{
    public class HelperController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
       
    }
}
