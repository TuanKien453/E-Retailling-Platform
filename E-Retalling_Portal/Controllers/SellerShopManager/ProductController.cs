using E_Retalling_Portal.Controllers.Filter;
using E_Retalling_Portal.Models;
using E_Retalling_Portal.Models.Enums;
using E_Retalling_Portal.Models.Query;
using E_Retalling_Portal.Services.ExtendService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.ComponentModel.DataAnnotations;
namespace E_Retalling_Portal.Controllers.ShopManager
{
    //[TypeFilter(typeof(ShopOwnerRoleFilter))]
    public class ProductController : Controller
    {
        public IActionResult Index()
        {

            return View("/Views/SellerShopManager/product/ViewProducts.cshtml");
        }

        public IActionResult EditVariation(int productId)
        {
            if (TempData["productId"] != null) {
                productId = (int)TempData["productId"];
            }
            else
            {
                TempData["productId"] = productId;
            }
            return View("/Views/SellerShopManager/product/EditVariation.cshtml");
        }

        public IActionResult AddProduct()
        {
            using (var context = new Context())
            {
                List<Category> categories = context.Categories.ToList();
                ViewBag.categories = BuildCategoryTree(categories);
            }
            return View("/Views/SellerShopManager/product/AddProduct.cshtml");
        }
        [HttpPost]
        public IActionResult AddProductProcess(Product product, List<IFormFile> img)
        {
            if (!ModelState.IsValid) {
                ModelState.ReadErrors();
                return View("Views/Shared/ErrorPage/Error500.cshtml");
            }

            int? accId = HttpContext.Session.GetInt32(SessionKeys.AccountId.ToString());
            //delete when intergration
            accId = 3;
            if (accId != null) {
                using (var context = new Context()) { 
                    var shop = context.Shops.GetShopbyAccId(accId.Value).FirstOrDefault();
                    product.shopId = shop.id;
                    product.status = 1;
                    context.Products.Add(product);
                    context.SaveChanges();
                    SaveImage(product.id,img,context);
                    //redicted to input more information
                    if (product.isVariation)
                    {
                        TempData["productId"] = product.id;
                        return RedirectToAction("EditVariation");
                    }
                    else
                    {
                        TempData["mess"] = "added success";
                        return RedirectToAction("Index");

                    }
                }
            }

            return View("Views/Shared/ErrorPage/Error500.cshtml");
        }

        private static void SaveImage(int id, List<IFormFile> img,Context context)
        {
            foreach (var file in img)
            {
                if (file != null && file.Length > 0)
                {
                    try
                    {
                        if (!Directory.Exists("images"))
                        {
                            Directory.CreateDirectory("images");
                        }

                        string uniqueFileName = GetUniquePath(file.FileName);
                        string filePath = Path.Combine("images", uniqueFileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            file.CopyTo(fileStream);
                        }
                        context.Images.Add(new Image{imagePath=filePath,productId=id });
                        context.SaveChanges();
                        Console.WriteLine($"File saved: {file.FileName} at {filePath}");
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Error saving file: " + ex.Message);
                    }
                }
            }
        }
        private static string GetUniquePath(string fileName)
        {
            string uniqueFileName = $"{DateTime.Now:yyyyMMddHHmmssfff}_{fileName}";

            return uniqueFileName;
        }
        private static List<Category> BuildCategoryTree(List<Category> list, int level = 0, int? parentid = null)
        {
            List<Category> result = new();
            foreach (var category in list.Where(c => c.parentCategoryId == parentid))
            {
                string newName = new string('\u00A0', level*2) + category.name;
                result.Add(new Category { id = category.id, name = newName, parentCategoryId = category.parentCategoryId });

                var children = BuildCategoryTree(list, level + 1, category.id);
                result.AddRange(children);
            }
            return result;
        }
    }
}
