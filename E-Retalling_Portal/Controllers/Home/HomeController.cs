using E_Retalling_Portal.Models;
using E_Retalling_Portal.Models.Enums;
using E_Retalling_Portal.Models.Query;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Drawing.Printing;
using System.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace E_Retalling_Portal.Controllers.Home
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
        public IActionResult Index(string searchQuery, int? categoryId, double? minPrice, double? maxPrice, int pageNumber = 1, int pageSize = 8)
        {
            using (var context = new Context())
            {
                var imageList = context.Images.ToList();
                var productList = context.Products.GetProduct().ToList();
                var categoryList = context.Categories.GetCategories().ToList();             
                var subcategoryList = context.Categories.GetSubCategories().ToList();       
                List<BreadcrumbItem> breadcrumbList = new List<BreadcrumbItem>();
                breadcrumbList = GetBreadcrumListFromCategoryList(categoryList, categoryId, breadcrumbList, context);

                ViewBag.breadcrumbList = breadcrumbList;

                //Get subcategory by categoryId
                if (categoryId.HasValue)
                {
                    ViewBag.isParentCategory = false;
                    ViewBag.subcategory = context.Categories.GetSubCategoriesByParentCategoryId(categoryId).ToList();
                }
                else
                {
                    ViewBag.isParentCategory = true;
                    ViewBag.categoryList = categoryList;
                }

                if (categoryId != null)
                {
                    //Get productList by filter category
                    productList = GetProductsBySubCategories(categoryId, categoryList, productList, context);
                }

                if (searchQuery != null)
                {
                    //Get product by filter search
                    productList = GetProductsByFilterSearch(productList, searchQuery);
                }

                if (minPrice != null || maxPrice != null)
                {
                    //Get product by filtery price
                    productList = productList = context.Products.GetProdutsByPrice(minPrice, maxPrice).ToList();
                }

                //Get min price of productItems
                if (productList != null)
                {
                    foreach (var product in productList)
                    {
                        if (product.isVariation == true && product.productItems.Count > 0)
                        {
                            var min = product.productItems.Min(item => item.price);

                            product.price = min;
                        }
                    }
                }
                List<Product> products = GetProductsIsNotDelete(productList);
                int totalProducts = products.Count();
                var paginatedProducts = products.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

                ViewBag.imageList = imageList;
                ViewBag.categoryId = categoryId;
                ViewBag.minPrice = minPrice;
                ViewBag.maxPrice = maxPrice;
                ViewBag.productList = paginatedProducts;
                ViewBag.totalPages = (int)Math.Ceiling((double)totalProducts / pageSize);
                ViewBag.currentPage = pageNumber;


                return View();
            }
        }

        private List<Product> GetProductsIsNotDelete(List<Product>? productList)
        {
            List<Product> products = new List<Product>();
            foreach (var product in productList)
            {
                if (product.isVariation == true)
                {
                    foreach (var productItem in product.productItems)
                    {
                        if (productItem.deleteAt == null)
                        {
                            products.Add(product);
                            break;
                        }
                    }
                }
                else
                {
                    products.Add(product);
                }
            }
            return products;
        }

        private List<BreadcrumbItem> GetBreadcrumListFromCategoryList(List<Category> categoryList, int? categoryId, List<BreadcrumbItem> breadcrumbList, Context context)
        {
            var currentCategory = context.Categories.GetSubCategoriesByCategoryId(categoryId).FirstOrDefault();

            while (currentCategory != null)
            {
                breadcrumbList.Insert(0, new BreadcrumbItem
                {
                    Name = currentCategory.name,
                    Url = currentCategory.id.ToString()
                });
                currentCategory = context.Categories.GetSubCategoriesByCategoryId(currentCategory.parentCategoryId).FirstOrDefault();
            }
            return breadcrumbList;
        }

        private List<Product> GetProductsByFilterSearch(List<Product> productList, string searchQuery)
        {
            if (!string.IsNullOrEmpty(searchQuery))
            {
                return productList.Where(p => p.name.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) && p.deleteAt != null)
                        .ToList(); ;
            }
            return productList;
        }


        private List<Product> GetProductsBySubCategories(int? categoryId, List<Category> categoryList, List<Product> productList, Context context)
        {
            if (!categoryId.HasValue)
            {
                return new List<Product>();
            }

            List<int> subCategoryIds = new List<int> { categoryId.Value };

            var childCategories = categoryList.Where(c => c.parentCategoryId == categoryId.Value && c.deleteAt == null).ToList();

            foreach (var category in childCategories)
            {
                subCategoryIds.Add(category.id);

                subCategoryIds.AddRange(GetProductsBySubCategories(category.id, categoryList, productList, context)
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