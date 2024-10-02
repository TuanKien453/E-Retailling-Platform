using Microsoft.AspNetCore.Mvc;

namespace E_Retalling_Portal.Controllers.ShopManager
{
    public class ProductController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult AddProduct()
        {
            return View("/Views/SellerShopManager/Product/AddProduct.cshtml");
        }
    }
}
