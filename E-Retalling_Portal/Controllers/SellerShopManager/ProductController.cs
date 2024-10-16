using E_Retalling_Portal.Controllers.Filter;
using E_Retalling_Portal.Models;
using E_Retalling_Portal.Models.Enums;
using E_Retalling_Portal.Models.Query;
using E_Retalling_Portal.Services.ExtendService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.ComponentModel.DataAnnotations;
namespace E_Retalling_Portal.Controllers.ShopManager
{
    [TypeFilter(typeof(ShopOwnerRoleFilter))]
    [TypeFilter(typeof(HaveShopFilter))]
    public class ProductController : Controller
    {
        public IActionResult Index()
        {

            return RedirectToAction("ViewProducts");
        }

        public IActionResult EditVariation(int productId)
        {

            ViewBag.productId = productId;
            using (var context = new Context())
            {
                //check valid product
                var product = context.Products.FirstOrDefault(p => p.id == productId && p.isVariation == true);
                if (product == null)
                {
                    return View("Views/Shared/ErrorPage/Error500.cshtml");
                }



                ViewBag.images = context.Images.GetImagesByProductId(productId).ToList();
                ViewBag.productItems = context.ProductItems.GetProductItem(productId).ToList();
            }
            return View("/Views/SellerShopManager/product/EditVariation.cshtml");
        }
        public IActionResult ViewProducts()
        {
            int? accId = HttpContext.Session.GetInt32(SessionKeys.AccountId.ToString());
            if (accId == null)
            {
                return View("Views/Shared/ErrorPage/Error500.cshtml");

            }

            using (var context = new Context())
            {
                var shop = context.Shops.GetShopbyAccId(accId.Value).FirstOrDefault();
                List<Product> products = context.Products.GetProductsByShop(shop.id).ToList();
                ViewBag.products = products;
            }

            return View("/Views/SellerShopManager/product/ViewProducts.cshtml");
        }

        public IActionResult AddProduct()
        {
            using (var context = new Context())
            {
                List<Category> categories = context.Categories.GetCategories().ToList();
                ViewBag.categories = BuildCategoryTree(categories);
            }
            return View("/Views/SellerShopManager/product/AddProduct.cshtml");
        }
        [HttpPost]
        public IActionResult AddProductProcess(Product product, List<IFormFile> img)
        {
            product.createAt = DateTime.Now.ToString();
            if (!ModelState.IsValid&&img.Count==0)
            {
                ModelState.ReadErrors();
                return View("Views/Shared/ErrorPage/Error500.cshtml");
            }

            int? accId = HttpContext.Session.GetInt32(SessionKeys.AccountId.ToString());
            if (accId != null)
            {
                using (var context = new Context())
                {
                    var shop = context.Shops.GetShopbyAccId(accId.Value).FirstOrDefault();
                    product.shopId = shop.id;
                    product.status = 1;
                    context.Products.Add(product);
                    context.SaveChanges();
                    SaveImage(img, context, product, true);
                    //redicted to input more information
                    if (product.isVariation)
                    {
                        return RedirectToAction("EditVariation", new { productId = product.id });
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

        public IActionResult AddVariation(ProductItem pi)
        {
            if (ModelState.IsValid)
            {
                ModelState.ReadErrors();
                using (var context = new Context())
                {
                    context.ProductItems.Add(pi);
                    context.SaveChanges();
                }
                return RedirectToAction("EditVariation", new { productId = pi.productId });
            }
            return View("Views/Shared/ErrorPage/Error500.cshtml");

        }
        public IActionResult EditProductItem(ProductItem pi)
        {
            if (ModelState.IsValid)
            {
                ModelState.ReadErrors();
                using (var context = new Context())
                {
                    context.ProductItems.Update(pi);
                    context.SaveChanges();
                }
                return RedirectToAction("EditVariation", new { productId = pi.productId });
            }
            return View("Views/Shared/ErrorPage/Error500.cshtml");

        }
        public IActionResult UpdateProduct(int Productid)
        {
            using (var context = new Context())
            {
                Product product = context.Products.GetProductById(Productid).FirstOrDefault();
                //check valid parameter
                if (product == null)
                {
                    return View("Views/Shared/ErrorPage/Error500.cshtml");
                }

                List<Category> categories = context.Categories.GetCategories().ToList();
                ViewBag.categories = BuildCategoryTree(categories);
                ViewBag.product = product;
            }
            return View("/Views/SellerShopManager/product/UpdateProduct.cshtml");
        }
        public IActionResult UpdateProductProcess(Product p, List<IFormFile> img)
        {
            Console.WriteLine("newImgCount:" + img.Count);
            if (!ModelState.IsValid)
            {

                ModelState.ReadErrors();
                return View("Views/Shared/ErrorPage/Error500.cshtml");
            }

            using (var context = new Context())
            {
                List<Image> imgs = context.Images.GetImagesByProductId(p.id).ToList();

                if (imgs.Count + img.Count > 6)
                {
                    return View("Views/Shared/ErrorPage/Error500.cshtml");
                }
                //excute update for exit image
                try
                {
                    foreach (var item in imgs)
                    {
                        bool isCovered = false;
                        if (item.productCoveredId != null)
                        {
                            isCovered = true;
                        }
                        var file = Request.Form.Files.GetFile("img" + item.id);
                        String isUpdate = Request.Form["isUpdate" + item.id];
                        //update img in this img id
                        if (file != null)
                        {
                            Console.WriteLine($"update image {item.id} to {file.FileName}");

                            //update data in file System
                            string uniqueFileName = GetUniquePath(file.FileName);
                            string filePath = Path.Combine("wwwroot/productImages", uniqueFileName);
                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                file.CopyTo(fileStream);
                            }
                            String exitImagePath = Path.Combine("wwwroot/productImages", item.imageName);
                            if (System.IO.File.Exists(exitImagePath))
                            {
                                System.IO.File.Delete(exitImagePath);
                            }

                            //update db
                            item.imageName = uniqueFileName;
                            context.Images.Update(item);
                            context.SaveChanges();
                        }
                        //delete image
                        else if (string.Equals(isUpdate, "true", StringComparison.OrdinalIgnoreCase) && !isCovered)
                        {
                            Console.WriteLine($"delete image {item.id}");
                            //delete in file system
                            String exitImagePath = Path.Combine("wwwroot/productImages", item.imageName);
                            if (System.IO.File.Exists(exitImagePath))
                            {
                                System.IO.File.Delete(exitImagePath);
                            }
                            //update db
                            context.Images.Remove(item);
                            context.SaveChanges();
                        }
                    }
                }
                catch (Exception ex) {
                    return View("Views/Shared/ErrorPage/Error500.cshtml");
                }
                    

                //new image
                SaveImage(img, context, p, false);
                //update product
                var product = context.Products.GetProductById(p.id).FirstOrDefault();
                product.name = p.name;
                product.price = p.price;
                product.quantity = p.quantity;
                product.categoryId = p.categoryId;
                product.desc = p.desc;
                context.SaveChanges();
            }



            return RedirectToAction("ViewProducts");
        }
        private static void SaveImage(List<IFormFile> img, Context context, Product addedProduct, Boolean setCover)
        {
            bool isFirstImg = true;

            foreach (var file in img)
            {
                if (file != null && file.Length > 0)
                {
                    try
                    {
                        if (!Directory.Exists("wwwroot/productImages"))
                        {
                            Directory.CreateDirectory("wwwroot/productImages");
                        }

                        string uniqueFileName = GetUniquePath(file.FileName);
                        string filePath = Path.Combine("wwwroot/productImages", uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            file.CopyTo(fileStream);
                        }

                        var newImg = new Image { imageName = uniqueFileName, productId = addedProduct.id };

                        if (setCover && isFirstImg)
                        {
                            newImg.productCoveredId = addedProduct.id;
                            isFirstImg = false;
                        }

                        context.Images.Add(newImg);
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
                string newName = new string('\u00A0', level * 2) + category.name;
                result.Add(new Category { id = category.id, name = newName, parentCategoryId = category.parentCategoryId });

                var children = BuildCategoryTree(list, level + 1, category.id);
                result.AddRange(children);
            }
            return result;
        }
    }
}
