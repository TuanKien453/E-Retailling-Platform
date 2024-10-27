using E_Retalling_Portal.Models.Enums;
using E_Retalling_Portal.Models;
using Microsoft.AspNetCore.Mvc;
using E_Retalling_Portal.Controllers.Filter;
using E_Retalling_Portal.Models.Query;
using E_Retalling_Portal.Services.ExtendService;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.CodeAnalysis;
using System.ComponentModel.DataAnnotations;
using Microsoft.Build.Evaluation;
namespace E_Retalling_Portal.Controllers.SellerShopManager
{
    public class DiscountManagerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ViewProductUnDiscount()
        {
            int? accId = HttpContext.Session.GetInt32(SessionKeys.AccountId.ToString());
            if (accId == null)
            {
                return View("Views/Shared/ErrorPage/Error500.cshtml");

            }

            using (var context = new Context())
            {
                var shop = context.Shops.GetShopbyAccId(accId.Value).FirstOrDefault();
                List<Product> products = new List<Product>();
                List<Discount> discounts = context.Discounts.GetDiscountByShop(shop.id).ToList();
                List<ProductDiscount> productDiscount = new List<ProductDiscount>();
                List<Tuple<int, int?>> productItemvalues = new List<Tuple<int, int?>>();
                if (!discounts.IsNullOrEmpty())
                {
                    foreach (var discount in discounts)
                    {

                        var items = context.ProductDiscount.GetProductDiscountByDiscountId(discount.id).ToList();
                        foreach (var item in items)
                        {
                            productDiscount.Add(item);
                        }
                    }
                }

                foreach (var discount in productDiscount)
                {
                    if (discount != null)
                    {
                        productItemvalues.Add(Tuple.Create(discount.productId, (int?)discount.productItemId));
                    }
                    
                }

                List<Product> productsStore = context.Products.GetProductsByShop(shop.id).ToList();
                List<Tuple<int, int?>> values = new List<Tuple<int, int?>>();
                foreach (var product in productsStore)
                {
                    List<ProductItem> items = context.ProductItems.GetProductItem(product.id).ToList();
                    if(items.IsNullOrEmpty())
                    {
                        values.Add(Tuple.Create(product.id, (int?)null));
                    }
                    else
                    {                        
                        foreach (var productItem in items)
                        {
                            values.Add(Tuple.Create(product.id, (int?)productItem.id));
                        }
                    }
                    
                }

                foreach (var productItemvalue in productItemvalues)
                {
                    if (values.Contains(productItemvalue))
                    {
                        values.Remove(productItemvalue);
                    }
                }

                List<Product> viewProducts = new List<Product>();
                foreach (var value in values)
                {
                    if (!viewProducts.Contains(context.Products.GetProductById(value.Item1).FirstOrDefault()))
                    {
                        viewProducts.Add(context.Products.GetProductById(value.Item1).FirstOrDefault());
                    }
                    
                }

                ViewBag.products = viewProducts;
            }

            return View("/Views/SellerShopManager/Discount/ViewProductUnDiscount.cshtml");
        }

        public IActionResult UnDiscountVariation(int productId)
        {
            int? accId = HttpContext.Session.GetInt32(SessionKeys.AccountId.ToString());
            if (accId == null)
            {
                return View("Views/Shared/ErrorPage/Error500.cshtml");

            }

            using (var context = new Context())
            {
                List<ProductItem> products = context.ProductItems.GetProductItem(productId).ToList();
                if (products.IsNullOrEmpty())
                {
                    return View("Views/Shared/ErrorPage/Error500.cshtml");
                }
                List<ProductDiscount> productDiscounts = context.ProductDiscount.GetProductDiscountByProductId(productId).ToList();
                List<ProductItem> productItemsDiscount = new List<ProductItem>();
                foreach (var discount in productDiscounts)
                {
                    if (discount.productItemId != null)
                    {
                        productItemsDiscount.Add(context.ProductItems.GetProductItemByProductItemId(discount.productItemId.Value).FirstOrDefault());

                    }

                }
                if (!productItemsDiscount.IsNullOrEmpty())
                {
                    foreach (var discount in productItemsDiscount)
                    {
                        if (products.Contains(discount))
                        {
                            products.Remove(discount);
                        }
                    }
                }
                

                ViewBag.images = context.Images.GetImagesByProductId(productId).ToList();
                ViewBag.productId = productId;
                ViewBag.productItems = products;

            }

            return View("/Views/SellerShopManager/Discount/UnDiscountVariation.cshtml");
        }

        public IActionResult ViewProductOnDiscount()
        {
            int? accId = HttpContext.Session.GetInt32(SessionKeys.AccountId.ToString());
            if (accId == null)
            {
                return View("Views/Shared/ErrorPage/Error500.cshtml");

            }

            using (var context = new Context())
            {
                var shop = context.Shops.GetShopbyAccId(accId.Value).FirstOrDefault();
                List<Product> products = new List<Product>();
                List<Discount> discounts = context.Discounts.GetDiscountByShop(shop.id).ToList();
                List<ProductDiscount> productDiscount = new List<ProductDiscount>();

                if (!discounts.IsNullOrEmpty())
                {
                    foreach (var discount in discounts)
                    {

                        var items = context.ProductDiscount.GetProductDiscountByDiscountId(discount.id).ToList();
                        foreach (var item in items)
                        {
                            productDiscount.Add(item);
                        }
                    }
                }

                foreach (var discount in productDiscount)
                {
                    if (discount != null && !products.Contains(context.Products.GetProductById(discount.productId).FirstOrDefault()))
                    {
                        products.Add(context.Products.GetProductById(discount.productId).FirstOrDefault());
                    } 
                }
                List<Discount> discountItem = new List<Discount>();
                foreach (var product in products)
                {
                    if (!product.isVariation)
                    {
                        ProductDiscount pd = context.ProductDiscount.GetProductDiscountByProductIdAndProductItemId(product.id, null).FirstOrDefault();
                        var discount = context.Discounts.GetDiscountByDiscountId(pd.discountId).FirstOrDefault();
                        discountItem.Add(discount);
                    }
                    else discountItem.Add(null);
                   
                }
                ViewBag.discount = discountItem;
                ViewBag.products = products;
            }

            return View("/Views/SellerShopManager/Discount/ViewProductOnDiscount.cshtml");
        }

        public IActionResult DiscountVariation(int productId)
        {
            int? accId = HttpContext.Session.GetInt32(SessionKeys.AccountId.ToString());
            if (accId == null)
            {
                return View("Views/Shared/ErrorPage/Error500.cshtml");

            }

            using (var context = new Context())
            {
                List<ProductItem> products = context.ProductItems.GetProductItem(productId).ToList();
                if (products.IsNullOrEmpty())
                {
                    return View("Views/Shared/ErrorPage/Error500.cshtml");
                }
                List<ProductDiscount> productDiscounts = context.ProductDiscount.GetProductDiscountByProductId(productId).ToList();
                List<ProductItem> productItemsDiscount = new List<ProductItem>();
                foreach (var discount in productDiscounts)
                {
                    if (discount.productItemId != null)
                    {
                  
                        productItemsDiscount.Add(context.ProductItems.GetProductItemByProductItemId(discount.productItemId.Value).FirstOrDefault());

                    }

                }

                List<Discount> discountItem = new List<Discount>();
                foreach (var productItem in productItemsDiscount)
                {
                    ProductDiscount pd = context.ProductDiscount.GetProductDiscountByProductIdAndProductItemId(productId, productItem.id).FirstOrDefault();
                    var discount = context.Discounts.GetDiscountByDiscountId(pd.discountId).FirstOrDefault();
                    discountItem.Add(discount);
                }

                ViewBag.images = context.Images.GetImagesByProductId(productId).ToList();
                ViewBag.productId = productId;
                ViewBag.productItems = productItemsDiscount;
                ViewBag.discount = discountItem;
            }

            return View("/Views/SellerShopManager/Discount/DiscountVariation.cshtml");
        }


        public IActionResult CreateDiscount()
        {
            return View("/Views/SellerShopManager/Discount/CreateDiscount.cshtml");
        }

        public IActionResult CreateDiscountProcess(Discount discount)
        {
            if (!ModelState.IsValid)
            {
                int? accId = HttpContext.Session.GetInt32(SessionKeys.AccountId.ToString());
                if (accId == null)
                {
                    return View("Views/Shared/ErrorPage/Error500.cshtml");

                }

                using (var context = new Context())
                {
                    var shop = context.Shops.GetShopbyAccId(accId.Value).FirstOrDefault();
                    discount.shopId = shop.id;
                    discount.deleteAt = null;
                    context.Discounts.Add(discount);
                    context.SaveChanges();
                }
                return RedirectToAction("ViewListDiscount");

            }
            else return View("Views/Shared/ErrorPage/Error500.cshtml");
        }

        public IActionResult ViewListDiscount()
        {
            int? accId = HttpContext.Session.GetInt32(SessionKeys.AccountId.ToString());
            if (accId == null)
            {
                return View("Views/Shared/ErrorPage/Error500.cshtml");

            }

            using (var context = new Context())
            {
                var shop = context.Shops.GetShopbyAccId(accId.Value).FirstOrDefault();
                List<Discount> discounts = context.Discounts.GetDiscountByShop(shop.id).ToList();
                ViewBag.discount = discounts;
            }
            return View("/Views/SellerShopManager/Discount/ViewListDiscount.cshtml");
        }

        public IActionResult DeleteDiscount(int discountId)
        {
            using (var context = new Context())
            {
               Discount? discount = context.Discounts.GetDiscountByDiscountId(discountId).FirstOrDefault();
                
                    context.Discounts.Remove(discount);
                    context.SaveChanges();
                    return RedirectToAction("ViewListDiscount");
                
            }
        }

        public IActionResult EditDiscount(int discountId)
        {
            using (var context = new Context())
            {
                Discount? discount = context.Discounts.GetDiscountByDiscountId(discountId).FirstOrDefault();
                if (discount != null)
                {
                    ViewBag.discount = discount;
                    TempData["discountId"] = discountId;
                    return View("/Views/SellerShopManager/Discount/EditDiscount.cshtml");
                }
                else return View("Views/Shared/ErrorPage/Error500.cshtml");
            }
        }

        [HttpPost]
        public IActionResult EditDiscountProcess(Discount discount)
        {
            int? accId = HttpContext.Session.GetInt32(SessionKeys.AccountId.ToString());
            if (accId == null) {
                return View("Views/Shared/ErrorPage/Error500.cshtml");
            }

            using (var context = new Context())
            {
                var shop = context.Shops.GetShopbyAccId(accId.Value).FirstOrDefault();
                discount.id = (int)TempData.Peek("discountId");
                discount.shopId = shop.id;
                discount.deleteAt = null;
                context.Discounts.Update(discount);
                context.SaveChanges();
                TempData.Remove("discountId");
                return RedirectToAction("ViewListDiscount");
            }
            
        }

        public IActionResult ChooseDiscountProduct(int productId, int? productItemId)
        {
            int? accId = HttpContext.Session.GetInt32(SessionKeys.AccountId.ToString());
            if (accId == null)
            {
                return View("Views/Shared/ErrorPage/Error500.cshtml");

            }

            using (var context = new Context())
            {
                var shop = context.Shops.GetShopbyAccId(accId.Value).FirstOrDefault();
                List<Discount> discounts = context.Discounts.GetDiscountByShop(shop.id).ToList();
                if (discounts.IsNullOrEmpty()) 
                {
                    return RedirectToAction("CreateDiscount");
                }
                TempData["productId"] = productId;
                TempData["productItemId"] = productItemId;
                ViewBag.discount = discounts;   
                return View("/Views/SellerShopManager/Discount/ChooseDiscountProduct.cshtml");
            }
            
        }
        public IActionResult ChooseDiscountProductProcess(int discountId)
        {
            using (var context = new Context())
            {
                ProductDiscount productDiscount = new ProductDiscount();
                productDiscount.productId = (int)TempData.Peek("productId");
                productDiscount.productItemId = TempData.Peek("productItemId") as int?;
                productDiscount.discountId = discountId;
                context.ProductDiscount.Add(productDiscount);
                context.SaveChanges();
                return RedirectToAction("ViewProductOnDiscount");
            }
        }

        public IActionResult ViewDetailDiscount(int productId, int? productItemId)
        {
            using (var context = new Context())
            {
                ProductDiscount productDiscount = context.ProductDiscount.GetProductDiscountByProductIdAndProductItemId(productId, productItemId).FirstOrDefault();
                var discount = context.Discounts.GetDiscountByDiscountId(productDiscount.discountId).FirstOrDefault();
                ViewBag.discount = discount;    
            }
            return View("/Views/SellerShopManager/Discount/ViewDetailDiscount.cshtml");
        }

        public IActionResult StopDiscount(int productId, int? productItemId)
        {
            using (var context = new Context())
            {
                ProductDiscount productDiscount = context.ProductDiscount.GetProductDiscountByProductIdAndProductItemId(productId, productItemId).FirstOrDefault();
                context.ProductDiscount.Remove(productDiscount);
                context.SaveChanges();
            }
            return RedirectToAction("ViewProductOnDiscount");
        }



    }
}
