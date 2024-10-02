using Microsoft.AspNetCore.Mvc;

namespace E_Retalling_Portal.Controllers.Manager
{
    public class ManagerDashBoardController : Controller
    {
        public IActionResult Index()
        {
            return View("/Views/ManagerSite/ManagerDashBoard/Index.cshtml");
        }
    }
}
