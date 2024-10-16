﻿using Microsoft.AspNetCore.Mvc;
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
                var product = context.Products.GetProductById(productId.Value).FirstOrDefault();
                var productItemList = context.ProductItems.GetProductItem(productId.Value).ToList();
                var similarProducts = context.Products.GetSimilarProductByProductCategory(product.category, product.id).ToList();
                if (product.isVariation == true)
                {
                    double minPrice = productItemList.Min(pi => pi.price);
                    double maxPrice = productItemList.Max(pi => pi.price);
                    ViewBag.minPrice = minPrice; ViewBag.maxPrice = maxPrice;
                }
                List<Product> products = GetProductsIsNotDelete(similarProducts);
                ViewBag.productImageList = product.images;
                ViewBag.product = product;
                Console.WriteLine(product.quantity+","+product.price);
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
