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
                var similarProducts = new List<Product>();
                if (product.category?.parentCategoryId.HasValue == true)
                {
                     similarProducts = context.Products
                        .GetSimilarProductByProductCategory(product.category.parentCategoryId.Value, product.id)
                        .ToList();
                }

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
                List<Product> products = Get6ProductsIsNotDelete(similarProducts);
                foreach (var item in products)
                {
                    if (item.isVariation == true && item.productItems.Count > 0)
                    {
                        var min = item.productItems.Min(item => item.price);

                        item.price = min;
                        Console.WriteLine(item.price);
                    }
                }
                var productDiscounts = context.ProductDiscounts.GetProductDiscount().ToList();
                ViewBag.productDiscounts = productDiscounts;
                ViewBag.quantityProduct = totalQuantityOfProductItems;
                ViewBag.productPrice = context.Products.GetProductDiscountPrice(product);
                ViewBag.productImageList = product.images;
                ViewBag.product = product;
                ViewBag.productItemList = productItemList;
                ViewBag.similarProducts = products;
            }
            return View("/Views/Home/ViewProductDetail.cshtml");
        }
        private List<Product> Get6ProductsIsNotDelete(List<Product>? productList)
        {
            List<Product> products = new List<Product>();
            int count = 0;
            foreach (var product in productList)
            {
                if (count >= 6)
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
