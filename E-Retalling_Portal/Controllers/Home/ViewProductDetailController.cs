using Microsoft.AspNetCore.Mvc;
using E_Retalling_Portal.Models;
using E_Retalling_Portal.Models.Query;
using E_Retalling_Portal.Models.Enums;
namespace E_Retalling_Portal.Controllers.Home
{
    public class ViewProductDetailController : Controller
    {
        public IActionResult Index(int? productId)
        {
            using (var context = new Context())
            {
                int? accountId = HttpContext.Session.GetInt32(SessionKeys.AccountId.ToString());
                var product = context.Products.GetProductById(productId.Value).FirstOrDefault();
                var productItemList = context.ProductItems.GetProductItem(productId.Value).ToList();
                var similarProducts = context.Products.GetSimilarProductByProductCategory(product.category,product.id).ToList();
                double minPrice = productItemList.Min(pi => pi.price);
                double maxPrice = productItemList.Max(pi => pi.price);

                ViewBag.minPrice = minPrice; ViewBag.maxPrice = maxPrice;
                ViewBag.accountId = accountId;
                ViewBag.product = product;
                ViewBag.productItemList = productItemList;
                ViewBag.similarProducts = similarProducts;
            }
            return View("/Views/Home/ViewProductDetail.cshtml");
        }
    }
}
