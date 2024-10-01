using Microsoft.AspNetCore.Mvc;

namespace E_Retalling_Portal.Controllers.Manager
{
    public class DashBoardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
