using Microsoft.AspNetCore.Mvc;
using E_Retalling_Portal.Models;
using E_Retalling_Portal.Models.Query;
using E_Retalling_Portal.Models.Enums;
using E_Retalling_Portal.Util;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Drawing.Printing;
using X.PagedList.Extensions;
namespace E_Retalling_Portal.Controllers.Home
{
    public class ViewProductDetailController : Controller
    {
        public IActionResult Index(int? productId, int? page)
        {
            CookiesUtils.SaveProductToCookie(productId.Value, Request, Response);
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
                    }
                }
                //---------------------------------
                var productDiscountItem = new Dictionary<int, ProductDiscountItemModel>();
                if (products != null)
                {
                    foreach (var product1 in products)
                    {
                        if (product1.isVariation == true && product1.productItems.Count > 0)
                        {
                            foreach (var item in product1.productItems)
                            {
                                if (!productDiscountItem.ContainsKey(product1.id))
                                {
                                    productDiscountItem[product1.id] = new ProductDiscountItemModel();
                                }
                                var productDiscount = context.ProductDiscounts.GetProductDiscountByProductId(product1.id).FirstOrDefault();
                                if (productDiscount != null)
                                {
                                    productDiscountItem[product1.id].productItem = item;
                                    productDiscountItem[product1.id].productDiscount = productDiscount;
                                    productDiscountItem[product1.id].discountedPrice = context.ProductItems.GetProductItemDiscountPrice(item);
                                    if (item.price != productDiscountItem[product1.id].discountedPrice)
                                    {
                                        productDiscountItem[product1.id].isDiscount = "true";
                                        break;
                                    }
                                    else
                                    {
                                        productDiscountItem[product1.id].isDiscount = "false";
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (!productDiscountItem.ContainsKey(product1.id))
                            {
                                productDiscountItem[product1.id] = new ProductDiscountItemModel();
                            }
                            var productDiscount = context.ProductDiscounts.GetProductDiscountByProductId(product1.id).FirstOrDefault();
                            if (productDiscount != null)
                            {
                                productDiscountItem[product1.id].product = product;
                                productDiscountItem[product1.id].productDiscount = productDiscount;
                                productDiscountItem[product1.id].discountedPrice = context.Products.GetProductDiscountPrice(product1);
                                if (product.price != productDiscountItem[product1.id].discountedPrice)
                                {
                                    productDiscountItem[product1.id].isDiscount = "true";
                                }
                                else
                                {
                                    productDiscountItem[product1.id].isDiscount = "false";
                                }
                            }
                        }
                    }
                }
                ViewBag.productDiscounts = productDiscountItem;
                foreach (var entry in productDiscountItem)
                {
                    // In key
                    Console.WriteLine($"Key: {entry.Key}");
                    Console.WriteLine($"Discounted Price: {entry.Value.isDiscount}");
                    // In các giá trị trong ProductDiscountItemModel
                    Console.WriteLine($"Discounted Price: {entry.Value.discountedPrice}");
                    if (entry.Value.product != null)
                    {
                        Console.WriteLine($"Product Price: {entry.Value.product.price}");
                    }
                    if (entry.Value.productItem != null)
                    {
                        Console.WriteLine($"productItem Price: {entry.Value.productItem.price}");
                    }
                    if (entry.Value.productDiscount != null)
                    {
                        Console.WriteLine($"Product Discount: {entry.Value.productDiscount.discount.value}");
                    }
                    Console.WriteLine("-----------------------------");
                }

                //---------------------------------------------------
                ViewBag.quantityProduct = totalQuantityOfProductItems;
                ViewBag.productPrice = context.Products.GetProductDiscountPrice(product);
                ViewBag.productImageList = product.images;
                ViewBag.product = product;
                ViewBag.productItemList = productItemList;
                ViewBag.similarProducts = products;
                var orderItems = context.OrderItems.GetStarAndCommentByProductId(product.id)
                                                    .Select(oi => new FeedbackViewModel
                                                    {
                                                        displayName = context.Users
                                                    .Where(u => u.id == oi.order.userId)
                                                    .Select(u => u.displayName)
                                                     .FirstOrDefault() ?? "",
                                                        productItemAttribute = oi.productItem.attribute,
                                                        comment = oi.comment,
                                                        rating = oi.rating
                                                    })
                                                    .OrderByDescending(oi => oi.rating)
                                                    .ToList() as List<FeedbackViewModel>;
                int pageSize = 5;
                int pageNumber = (page ?? 1);
                var feedbackList = orderItems.ToPagedList(pageNumber, pageSize);
                var ratings = context.OrderItems
                                                    .Where(oi => oi.productId == productId && oi.rating.HasValue)
                                                    .Select(oi => oi.rating.Value)
                                                    .ToList();

                int averageRating = ratings.Any() ? (int)Math.Round(ratings.Average()) : 0;
                ViewBag.averageRating = averageRating;

                return View("/Views/Home/ViewProductDetail.cshtml", feedbackList);
            }
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
