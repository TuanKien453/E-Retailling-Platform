using E_Retalling_Portal.Controllers.Filter;
using E_Retalling_Portal.Models;
using E_Retalling_Portal.Models.Query;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
namespace E_Retalling_Portal.Controllers.ShopManager
{
    //[TypeFilter(typeof(ShopOwnerRoleFilter))]
    public class ProductController : Controller
    {
        public IActionResult Index()
        {

            return View();
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
        public IActionResult AddProductProcess(Product product)
        {
            if (!ModelState.IsValid) {
                return View("Views/Shared/ErrorPage/Error500.cshtml");
            }

            //redicted to input more information
            if (product.isVariation)
            {
                return View();
            }
            else
            {
                TempData["mess"] = "added success";
                return RedirectToAction("Index");
                
            }



        }




        private List<Category> BuildCategoryTree(List<Category> list, int level = 0, int? parentid = null)
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
