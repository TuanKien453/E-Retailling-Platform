using Microsoft.AspNetCore.Mvc;
using E_Retalling_Portal.Models;
using E_Retalling_Portal.Models.Query;
namespace E_Retalling_Portal.Controllers.Home
{
    public class ViewProductDetailController : Controller
    {
        public IActionResult Index(int productId)
        {
            using (var context = new Context())
            {
                var coverImage = context.Images.GetCoverImagesByProductId(productId).FirstOrDefault();
                var imageList = context.Images.GetImagesByProductId(productId).ToList();
                var product = context.Products.GetProductById(productId).FirstOrDefault();
                var productItemList = context.ProductItems.GetProductItem(productId).ToList();

                ViewBag.coverImage = coverImage;
                ViewBag.imageList = imageList;
                ViewBag.product = product;
                ViewBag.productItemList = productItemList;
            }
            return View("/Views/Home/ViewProductDetail.cshtml");
        }
    }
}
