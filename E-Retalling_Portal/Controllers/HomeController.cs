using E_Retalling_Portal.Models;
using E_Retalling_Portal.Models.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace E_Retalling_Portal.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Error505()
        {
            return View("Views/Shared/ErrorPage/Error500.cshtml");
        }
        public IActionResult Index(string searchQuery, int? categoryId, double? minPrice, double? maxPrice)
        {
            using (var context = new Context())
            {
                var imageList = context.Images.ToList();
                var productList = context.Products.ToList();
                var categoryList = context.Categories.ToList();
                var subcategoryList = context.Categories.Where(c => c.parentCategoryId != null).ToList();
                List<BreadcrumbItem> breadcrumbList = new List<BreadcrumbItem>();

                breadcrumbList = GetBreadcrumListFromCategoryList(categoryList, categoryId, breadcrumbList);

                ViewBag.breadcrumbList = breadcrumbList;

                //Get subcategory by categoryId
                if (categoryId.HasValue)
                {
                    ViewBag.isParentCategory = false;
                    ViewBag.subcategory = subcategoryList.Where(c => c.parentCategoryId == categoryId).ToList();
                }
                else
                {
                    ViewBag.isParentCategory = true;
                    ViewBag.categoryList = categoryList;
                }

                if (categoryId != null)
                {
                    //Get productList by filter category
                    productList = GetProductsBySubCategories(categoryId, categoryList, productList);
                }

                //Map images into product by productId
                var productImageMap = productList.ToDictionary(
                    product => product.id,
                    product => imageList.Where(img => img.productId == product.id).ToList()
                );

                if (searchQuery != null)
                {
                    //Get product by filter search
                    productList = GetProductsByFilterSearch(productList, searchQuery);
                }

                if (minPrice != null || maxPrice != null)
                {
                    //Get product by filtery price
                    productList = GetProductsByFilterPrice(productList, minPrice, maxPrice);
                }

                ViewBag.categoryId = categoryId;
                ViewBag.minPrice = minPrice;
                ViewBag.maxPrice = maxPrice;
                ViewBag.ProductImageMap = productImageMap;
                ViewBag.productList = productList;
                return View();
            }
        }

        private List<BreadcrumbItem> GetBreadcrumListFromCategoryList(List<Category> categoryList, int? categoryId, List<BreadcrumbItem> breadcrumbList)
        {
            var currentCategory = categoryList.FirstOrDefault(c => c.id == categoryId);

            while (currentCategory != null)
            {
                breadcrumbList.Insert(0, new BreadcrumbItem
                {
                    Name = currentCategory.name,
                    Url = currentCategory.id.ToString()
                });
                currentCategory = categoryList.FirstOrDefault(c => c.id == currentCategory.parentCategoryId);
            }
            return breadcrumbList;
        }

        private List<Product> GetProductsByFilterSearch(List<Product> productList, string searchQuery)
        {
            if (!string.IsNullOrEmpty(searchQuery))
            {
                return productList.Where(p => p.name.Contains(searchQuery, StringComparison.OrdinalIgnoreCase))
                        .ToList(); ;
            }
            return productList;
        }
        private List<Product> GetProductsByFilterPrice(List<Product> productList, double? minPrice, double? maxPrice)
        {
            if (minPrice.HasValue && maxPrice.HasValue)
            {
                if (minPrice >= 2 && maxPrice < 1000)
                {
                    productList = productList
                        .Where(p => p.price >= minPrice.Value && p.price < maxPrice.Value)
                        .ToList();
                }
                else
                if (minPrice >= 2 && maxPrice >= 1000)
                {
                    productList = productList
                        .Where(p => p.price >= minPrice.Value)
                        .ToList();
                }
            }
            return productList;
        }



        private List<Product> GetProductsBySubCategories(int? categoryId, List<Category> categoryList, List<Product> productList)
        {
            if (!categoryId.HasValue)
            {
                return new List<Product>();
            }

            List<int> subCategoryIds = new List<int> { categoryId.Value };

            var childCategories = categoryList.Where(c => c.parentCategoryId == categoryId.Value).ToList();

            foreach (var category in childCategories)
            {
                subCategoryIds.Add(category.id);

                subCategoryIds.AddRange(GetProductsBySubCategories(category.id, categoryList, productList)
                    .Select(p => p.categoryId));
            }

            return productList.Where(p => subCategoryIds.Contains(p.categoryId)).ToList();
        }




        public IActionResult Privacy()
        {
            return View();
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}


//@functions {
//    private void DisplaySubCategories(IEnumerable<Category> subCategories)
//{
//    foreach (var subCategory in subCategories)
//    {
//            < li style = "margin-left: 20px" >
//                < input type = "checkbox"
//                       id = "subcategory_@subCategory.id"
//                       name = "selectedSubCategories"
//                       value = "@subCategory.id"
//                @(ViewBag.selectedSubCategories.Contains(subCategory.id) ? "checked" : "") />

//            < label for= "subcategory_@subCategory.id" > @subCategory.name </ label >

//        </ li >

//            if (subCategory.childrens != null && subCategory.childrens.Any())
//            {
//                DisplaySubCategories(subCategory.childrens);
//            }
//    }
//}
//}
//                                        @{
//    DisplaySubCategories(category.childrens);
//}