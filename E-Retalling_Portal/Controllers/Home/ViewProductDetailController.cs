using Microsoft.AspNetCore.Mvc;
using E_Retalling_Portal.Models;
using E_Retalling_Portal.Models.Query;
using E_Retalling_Portal.Models.Enums;
using E_Retalling_Portal.Util;
using PagedList;
namespace E_Retalling_Portal.Controllers.Home
{
    public class ViewProductDetailController : Controller
    {
        public IActionResult Index(int? productId)
        {
            CookiesUtils.SaveProductToCookie(productId.Value,Request,Response);
            using (var context = new Context())
            {
                var product = context.Products.GetProductById(productId.Value).FirstOrDefault();
                var productItemList = context.ProductItems.GetProductItem(productId.Value).ToList();
                var similarProducts = context.Products.GetSimilarProductByProductCategory(product.category, product.id).ToList();
                int totalQuantityOfProductItems = 0;

                if (product.isVariation == true)
                {
                    
                    
                    Dictionary<int, double> discountPrices = new Dictionary<int, double>();
                    foreach (var item in productItemList)
                    {
                        totalQuantityOfProductItems += item.quantity;
                            var discountedPrice = context.ProductItems.GetProductItemDiscountPrice(item);
                            discountPrices[item.id] = discountedPrice;
                    }
                    var maxPrice = discountPrices.Min(pd => pd.Value);
                    var minPrice = discountPrices.Max(pd => pd.Value);
                    foreach (var item in discountPrices)
                    {
                        if (item.Value < minPrice)
                        {
                            minPrice = item.Value;
                        }
                        if (item.Value > maxPrice)
                        {
                            maxPrice = item.Value;
                        }
                    }
                    


                    ViewBag.discountPrices = discountPrices;
                    ViewBag.minPrice = minPrice;
                    ViewBag.maxPrice = maxPrice;
                }
                List<Product> products = GetProductsIsNotDelete(similarProducts).Take(6).ToList();

                Dictionary<int, double> discountProductSimilar = new Dictionary<int, double>();
                foreach(var item in products)
                {
                    var discountPrice = context.Products.GetProductDiscountPrice(item);
                    discountProductSimilar[item.id] = discountPrice;
                }

                ViewBag.discountProductSimilar = discountProductSimilar;
                ViewBag.quantityProduct = totalQuantityOfProductItems;
                ViewBag.productPrice = context.Products.GetProductDiscountPrice(product);
                ViewBag.productImageList = product.images;
                ViewBag.product = product;
                ViewBag.productItemList = productItemList;
                ViewBag.similarProducts = products;
            }
            return View("/Views/Home/ViewProductDetail.cshtml");
        }
        private List<Product> GetProductsIsNotDelete(List<Product>? productList)
        {
            List<Product> products = new List<Product>();
            int count = 0;
            foreach (var product in productList)
            {
                if (count >= 10)
                {
                    break;
                }
                if (product.isVariation == true)
                {
                    foreach (var productItem in product.productItems)
                    {
                        if (productItem.deleteAt == null)
                        {
                            products.Add(product);
                            count++;
                            break;
                        }
                    }
                }
                else
                {
                    products.Add(product);
                    count++;
                }
            }
            return products;
        }
    }
}
