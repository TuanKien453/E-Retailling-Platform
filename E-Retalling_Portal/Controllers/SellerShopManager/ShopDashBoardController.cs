using Microsoft.AspNetCore.Mvc;

namespace E_Retalling_Portal.Controllers.ShopManager
{
    public class ShopDashBoardController : Controller
    {
        public IActionResult Index()
        {
            return View("/Views/SellerShopManager/ShopDashBoard/Index.cshtml");
        }
    }
}
