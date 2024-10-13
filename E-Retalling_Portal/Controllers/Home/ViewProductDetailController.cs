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
                var coverImage = context.Images.GetCoverImagesByProductId(productId.Value).FirstOrDefault();
                var imageList = context.Images.GetImagesByProductId(productId.Value).ToList();
                var product = context.Products.GetProductById(productId.Value).FirstOrDefault();
                var productItemList = context.ProductItems.GetProductItem(productId.Value).ToList();
                var similarProducts = context.Products.GetSimilarProductByProductCategory(product.category).ToList();

                ViewBag.accountId = accountId;
                ViewBag.coverImage = coverImage;
                ViewBag.imageList = imageList;
                ViewBag.product = product;
                ViewBag.productItemList = productItemList;
                ViewBag.similarProducts = similarProducts;
            }
            return View("/Views/Home/ViewProductDetail.cshtml");
        }
    }
}
